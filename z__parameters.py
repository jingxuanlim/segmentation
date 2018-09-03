
# INPUT VARIABLES:
# - code_dir: directory in which the code resides
# - func_dir: an optional variable which defines input and output directories
# - input_dir: input directory
# - output_dir: output directory
# - xml_filename: full path to xml filename that has recording metadata
# - stack_filename: full path to stack filename that has recording metadata
# - alignment_type: type of motion correction.
#     possible values:
#       translation: (translation only)
#       rigid: (translation and rotation)
# - dt: the range of timepoints to use for cell detection
#     can be a vector which specifies the precise point sequence
#     can be a scalar which specifies the downsampling factor
#     e.g.
#       dt = list(range(2000))      # first 2000 timepoints of recording
#       dt = 10;                    # every 10th point of recording
#       dt = 1, dt = 0, dt = []     # every point of recording
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

code_dir        = ''
func_dir        = ''
input_dir       = '' + func_dir
output_dir      = '' + func_dir + '/ana/'
xml_filename    = input_dir + '/ch0.xml'
stack_filename  = input_dir + '/Stack_frequency.txt'
alignment_type  = 'rigid'
dt              = 1
thr_mask        = 0
ds              = 2
blok_cell_nmbr  = 100
cell_diam       = 6.0
imageframe_nmbr = 1

# packed planes: set to 1 when single plane stacks are packed into a 3d-volume
packed_planes = 0

# configure parameters, function, variable and path definitions
from past.builtins import execfile
execfile(code_dir + 'zfun.py')
execfile(code_dir + 'z0_preliminaries.py')
os.chdir(output_dir)
