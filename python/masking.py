cd /groups/ahrens/ahrenslab/mika/chen/20171103/L5-561nm-Highfrequency_20171103_173803.corrected-CM00/ana

##

imagename = '/nrs/keller/mika/20171103/A9 basin cell Mask.tif'
imagename = '/nrs/keller/mika/20171103/L5-488nm_20171103_163914.corrected_short/SPM00/TM000000/SPM00_TM000000_CM00_CHN00.klb'
imagename = '/nrs/keller/mika/20171103/L5-488nm_20171103_163914.corrected_short/SPM00/TM000000/SPM00_TM000000_CM01_CHN00.klb'

try:
    image_data = tifffile.imread(imagename)
except ValueError:
    image_data = pyklb.readfull(imagename)

image_data = image_data.transpose(1, 2, 0).astype(data_type)
image_data = downsample_xy(image_data)
# image_data
image_data = pad_z('pad', image_data)

nibabel.save(
    nii_image(image_data.astype(data_type), niiaffmat),
    '/groups/ahrens/ahrenslab/mika/chen/20171103/' \
    + imagename.split('/')[-1].split('.')[0].replace(' ', '_') + '.nii.gz')

##

%cd /groups/ahrens/home/rubinovm/mycode/zfish_prepro/code
from zfun import ants_registration, ants_transformation

%cd /groups/ahrens/ahrenslab/mika/chen/20171103
for i in [ \
    './L5-561nm-Highfrequency_20171103_173803.corrected-CM00/ana/',
    './L5-561nm-Highfrequency_20171103_173803.corrected-CM01/ana/',
    './L5-561nm-ROImonitoring_20171103_164908.corrected-CM00/ana/',
    './L5-561nm-ROImonitoring_20171103_164908.corrected-CM01/ana/']:
    
    j = i.split('/')[-3][-1]
    print(i, j)
    
    ants_registration(\
        in_nii = './basin_cell_mask/SPM00_TM000000_CM0' + j + '_CHN00.nii.gz',
        ref_nii = i + 'image_reference_aligned0.nii.gz',
        out_nii = i + 'basin_cell_image_reference0.nii.gz',
        out_tform = i + 'basin_cell_tform_',
        tip = 'ra')
        
    ants_transformation(\
        in_nii = './basin_cell_mask/A9_basin_cell_Mask.nii.gz',
        ref_nii = i + 'image_reference_aligned0.nii.gz',
        out_nii = i + 'basin_cell_mask.nii.gz',
        in_tform = i + 'basin_cell_tform_0GenericAffine.mat',
        interpolation = 'NearestNeighbor')

