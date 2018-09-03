from past.builtins import execfile

code_dir = '/groups/ahrens/home/rubinovm/mycode/zfish_prepro/code/'

# input and output directories

func_dir = 'L1-561nm-ROImonitoring_20171103_105014.corrected'
input_dir  = '/nrs/keller/mika/' + func_dir + '/Results/WeightFused/'
output_dir = '/groups/ahrens/ahrenslab/mika/chen/CW_17-11-03' + func_dir + '/ana/'

execfile(code_dir + 'zfun.py')

# downsampling parameters, padding width, alignment cycles, cell numbers
xml_filename      = input_dir + '/../../ch0.xml'              # xml filename
stack_filename    = input_dir + '/../../TM elapsed times.txt' # stack filename
alignment_cycles  = 1                                   # number of alignment cycles
lpad              = 0                                   # padding in z
cell_diam         = 4.0                                 # cell diameter (microns)
blok_cell_nmbr    = 100.0                               # average number of cells in block

# these parameters are usually unchanged
imageframe_nmbr   = 1                                   # no. brains in each image (1 or 2)
ds                = 1                                   # x and y dimension downsample factor
dt                = 1                                   # temporal downsample factor
interactivity     = 1                                   # interactivity for selecting brain mask

# output file formats
data_type         = 'float32'
nii_ext           = '.nii.gz'
image_ext         = '.klb'                              # output file formats (check image extension)

# image names: check that the names are correct         # '/' + input_dir.strip('/') + '0'
imagename_part    = 'TM'                               # part of name used to identify images
def get_image_names(all_filenames, name_part=imagename_part):
    # return ['.'.join(name.split('.')[:-1]) for name in all_filenames if name_part in name]
    return [name for name in all_filenames if name_part in name]

image_names = get_image_names(os.listdir(input_dir))
image_names.sort()

# get spatial and temporal parameters
resn_x, resn_y, resn_z, _, _, _, t_exposure, t_stack, freq_stack, freq_ephys \
    = parse_info(xml_filename, stack_filename, imageframe_nmbr)

def klb2hdf(image_name):
    print(image_name)

    in_filename = image_name + '/SPM00_' + image_name + \
                      '_CM00_CM01_CHN00.weightFused.TimeRegistration.klb'
    ou_filename = output_dir + '/brain_images/0/' + image_name + '/image_aligned.hdf5'
    
    os.system('mkdir -p ' + '/'.join(ou_filename.split('/')[:-1]))
    with h5py.File(ou_filename, 'w') as file_handle:
        image_data = pyklb.readfull(input_dir + in_filename)
        image_data = image_data.transpose(0, 2, 1).astype(data_type)
        image_mean = image_data[()].mean(dtype='float64')
        
        lz, ly, lx = image_data.shape
        file_handle.create_dataset("V3D", (lz, ly, lx), dtype=data_type, compression="gzip")
        file_handle['V3D'][()] = image_data
    
    return (image_mean, (lx, ly, lz))
    
image_info = sc.parallelize(image_names).map(klb2hdf).collect()
image_mean, image_lxyz = [np.array(i) for i in zip(*image_info)]
assert(~image_lxyz.ptp(0).any())
lx, ly, lz = image_lxyz[0]
pp.plot(image_mean); pp.show()

with h5py.File(output_dir + 'image_reference_thrs_time0.hdf5', 'w') as file_handle:
    file_handle['image_mean'] = image_mean

# get number of timepoints and estimate manipulation-free dt_range
lt = len(image_names)
dt_range = np.r_[:lt:dt]
stim_tims = np.r_[1590, 2400, 1603, 5030]
dt_range = np.setdiff1d(dt_range, (stim_tims[None] + np.r_[-10:40][:, None]).ravel())
pp.plot(image_mean); pp.plot(dt_range, image_mean[dt_range], '.'); pp.show()

print([lt, len(dt_range)])
print((lx, ly, lz, lt, ds, dt, lpad))

# preliminaries (parameters, function, variable and path definitions)
execfile(code_dir + 'z0_preliminaries.py')
