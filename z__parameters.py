
# INPUT VARIABLES:
# - code_dir: directory in which the code resides
# - func_dir: an optional variable which defines input and output directories
# - input_dir: input directory
# - output_dir: output directory
# - xml_filename: full path to xml filename that has recording metadata
# - stack_filename: full path to stack filename that has recording metadata
# - dt_range: the range of timepoints to use for cell detection
#     can be a vector which specifies the precise point sequence
#     can be a scalar which specifies the downsampling factor
#     e.g. 
#       dt_range = np.r_[0:2000]  # first 2000 timepoints of recording
#       dt_range = 10;            # every 10th point of recording
# - thr_mask: fluorescence threshold for brain masking the brain
#     typical values lie in the range of 100 to 110
#     if in doubt, set thr_mask=0 (this allows to choose it interactively later)
#     NB: cell detection is only performed on suprathreshold pixels
# - ds: spatial coarsegraining of in x-y dimensions:
#     typical value is 2 (leading to 4-fold reduction in size)
#     set value to 1 to maintain original dimensions
# - blok_cell_nmbr: number of cells in each cell_detection blok
#     larger values result in larger blocks and slower computation
# - cell_diam: approximate cell diameter
# - imageframe_nmbr: number of brains in each image
#     typical value is 1.
#     value is 2 if two-color image

code_dir        = '/groups/ahrens/home/rubinovm/mycode/zfish_prepro/code/'
func_dir        = '/ablation_ALMA/2015-07-23fish2_6dpf/dOMR0_20150723_150907/'
input_dir       = '/groups/ahrens/ahrenslab/FROM_TIER2/Nikita/' + func_dir
output_dir      = '/groups/ahrens/ahrenslab/mika/nikita/' + func_dir + '/ana/'
xml_filename    = input_dir + '/ch0.xml'
stack_filename  = input_dir + '/Stack_frequency.txt'
dt_range        = []
thr_mask        = 0
ds              = 2
blok_cell_nmbr  = 100
cell_diam       = 6.0
imageframe_nmbr = 1

# output file formats (typically no need to change)
data_type = 'float32'
nii_ext   = '.nii.gz'

# packed planes: set to 1 when single plane stacks are packed into a 3d-volume
packed_planes = 0

# configure parameters, function, variable and path definitions
from past.builtins import execfile
execfile(code_dir + 'zfun.py')
execfile(code_dir + 'z0_preliminaries.py')
os.chdir(output_dir)
