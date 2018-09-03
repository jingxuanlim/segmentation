# navigate to the output directory first before executing this block
try:
    import h5py
    with h5py.File('prepro_parameters.hdf5', 'r') as file_handle:
        for key in file_handle:
            var = file_handle[key][()]
            if isinstance(var, str):
                try: var = var.decode()
                except AttributeError: pass
            elif var.size > 1:
                try: var = [i.decode() for i in var]
                except AttributeError: pass                
            exec(key + '=var')
    print('Successfully imported from prepro_parameters.hdf5')
except:
    pass

##

# get_ipython().run_line_magic('matplotlib', 'inline')
from past.builtins import execfile
execfile(code_dir + 'zfun.py')
execfile(code_dir + 'zfun_cell.py')

## actual preprocessing begins here ##

# 1. alignment (motion correction)
execfile(code_dir + 'z1_alignment.py')

# 2. series conversion
execfile(code_dir + 'z2_brain_mask.py')

# 3. cell detection
execfile(code_dir + 'z3_cell_detect.py')

# 4. cell collection into a single file
execfile(code_dir + 'z4_cell_collect.py')

# shutdown spark job
os.system('spark-janelia-lsf stopcluster -f')

# find . -name "image_aligned*" -delete
# find . -name "Block*hdf5" -delete
