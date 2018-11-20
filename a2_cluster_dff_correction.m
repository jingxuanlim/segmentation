%%

addpath /groups/ahrens/home/rubinovm/NIfTI_20140122/
clear variables
output_dir = [pwd '/'];
cd(output_dir)

% number of clusters
K = [500];

%%

imageframe_nmbr = h5read([output_dir 'prepro_parameters.hdf5'], '/imageframe_nmbr');
for frame_i = 0:imageframe_nmbr-1
    %%
    
    load(['Cells' num2str(frame_i) '_clean.mat'], 'Cell_X', 'Cell_Y', 'Cell_Z', 'Cell_spcesers');
    load(['Cells' num2str(frame_i) '_baseline.mat'], 'Cell_timesers1', 'Cell_baseline1');
    nii = load_nii(['image_reference_aligned' num2str(frame_i) '.nii.gz']);
    l = h5read('./prepro_parameters.hdf5', '/lpad');
    img = nii.img(:, :, l+1:end-l);
    c = 80;
    
    D = zeros(size(Cell_timesers1));
    for i = 1:size(Cell_X, 1)
        ti = Cell_timesers1(i, :);
        bi = Cell_baseline1(i, :);
        
        ni = nnz(isfinite(Cell_X(i, :)));
        pi = zeros(1, ni);
        for j = 1:ni
            pi(j) = img(Cell_X(i, j), Cell_Y(i, j), Cell_Z(i, j));
        end
        
        mi = sum(Cell_spcesers(i, 1:ni).*pi)./sum(Cell_spcesers(i, 1:ni));
        D(i, :) = (ti - bi) ./ (bi + (1 - mean(ti) - c/mi));
    end
    D(D < 0) = 0;
    
    [W0, H0] = nndsvd_econ(D);
    
    %%
    
    CellW = cell(1, numel(K));
    CellH = cell(1, numel(K));
    for i = 1:numel(K)
        k = K(i);
        disp(k)
        [CellW{i}, CellH{i}] = nmfh_lite(D, H0(1:k, :), 100, 100, 1e-4);
        
        save(['Cells_dff' num2str(frame_i) '_clust.mat'], '-v7.3');
    end
end
