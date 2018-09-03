# configure only if prepro_parameters doesn't exist
if os.path.isfile(output_dir + 'prepro_parameters.hdf5'):
    sys.exit('Imported modules and exiting')
    
# get image extension and image names
file_names = [i.split('.', 1) for i in os.listdir(input_dir)]
file_names = list(itertools.zip_longest(*file_names, fillvalue=''))
file_exts, counts = np.unique(file_names[1], return_counts=True)
image_ext = file_exts[np.argmax(counts)]
image_names = [i for i, j in zip(*file_names) if j==image_ext]
image_names.sort()

# unpack single planes
if packed_planes:
    input_dir0 = input_dir
    input_dir += 'pln/'
    os.system('mkdir ' + input_dir)
    def volume_to_singleplane(image_name):
        with h5py.File(input_dir0 + image_name + '.h5', 'r') as f:
            vol = f['default'][()]
            
        for i, vol_i in enumerate(vol):
            with h5py.File(input_dir + image_name + '_PLN' + str(i).zfill(2) + '.h5', 'w') as g:
                g['default'] = vol_i
    
    sc.parallelize(image_names).foreach(volume_to_singleplane)
    image_names = get_image_names(os.listdir(input_dir))
    image_names.sort()

# get number of timepoints
lt = len(image_names)

# get spatial and temporal parameters
resn_x, resn_y, resn_z, lx, ly, lz, t_exposure, t_stack, freq_stack \
    = parse_info(xml_filename, stack_filename, imageframe_nmbr)

# in case of packed planes, modify lz and freq_stack/t_stack
if packed_planes:
    freq_stack *= lz
    t_stack /= lz
    lz = 1;

# if single plane, adjust resolution and padding
if lz==1:
    lpad = 0
    resn_z = 1.0
else:
    lpad = 4

niiaffmat = np.diag([resn_x * ds, resn_y * ds, resn_z, 1])
cell_ball_fine, cell_ball_midpoint_fine = get_ball(0.5 * cell_diam)
cell_ball,      cell_ball_midpoint      = get_ball(1.0 * cell_diam)

# get number of voxels in each cell
if (lz == 1) or (resn_z >= cell_diam):
    cell_voxl_nmbr = np.pi * (np.square(cell_diam / 2.0)) / (resn_x * ds * resn_y * ds)
else:
    cell_voxl_nmbr = \
        cell_diam * np.pi * (np.square(cell_diam / 2.0)) / (resn_x * ds * resn_y * ds * resn_z)
        

# make directories and save  parameters
os.system('mkdir -p ' + output_dir + '{brain_images,cell_series}')
with h5py.File(output_dir + 'prepro_parameters.hdf5', 'w') as file_handle:
    file_handle['cell_ball']               = cell_ball
    file_handle['cell_ball_fine']          = cell_ball_fine
    file_handle['cell_ball_midpoint']      = cell_ball_midpoint
    file_handle['cell_ball_midpoint_fine'] = cell_ball_midpoint_fine
    file_handle['cell_diam']               = cell_diam
    file_handle['cell_voxl_nmbr']          = cell_voxl_nmbr
    file_handle['code_dir']                = code_dir
    file_handle['blok_cell_nmbr']          = blok_cell_nmbr
    file_handle['data_type']               = data_type
    file_handle['ds']                      = ds
    file_handle['dt_range']                = dt_range
    file_handle['freq_stack']              = freq_stack
    file_handle['image_ext']               = image_ext
    file_handle['image_names']             = image_names
    file_handle['imageframe_nmbr']         = imageframe_nmbr
    file_handle['input_dir']               = input_dir
    file_handle['lpad']                    = lpad
    file_handle['lt']                      = lt
    file_handle['lx']                      = lx
    file_handle['ly']                      = ly
    file_handle['lz']                      = lz
    file_handle['nii_ext']                 = nii_ext
    file_handle['niiaffmat']               = niiaffmat
    file_handle['output_dir']              = output_dir
    file_handle['resn_x']                  = resn_x
    file_handle['resn_y']                  = resn_y
    file_handle['resn_z']                  = resn_z
    file_handle['t_stack']                 = t_stack
    file_handle['t_exposure']              = t_exposure
    file_handle['thr_mask']                = thr_mask