clear variables
output_dir = [pwd '/'];
cd(output_dir)

%%

K = [20 60];

imageframe_nmbr = h5read([output_dir 'prepro_parameters.hdf5'], '/imageframe_nmbr');

%%

for frame_i = 0:imageframe_nmbr-1
    %%
    
    load(['Cells' num2str(frame_i) '_baseline.mat'], 'Cell_timesers1', 'Cell_baseline1');
    
    T = (Cell_timesers1 - Cell_baseline1);
    T(T < 0) = 0;
    [W0, H0] = nndsvd_econ(T);
    
    %%
    
    CellW = cell(1, numel(K));
    CellH = cell(1, numel(K));
    for i = 1:numel(K)
        k = K(i);
        disp(k)
        [CellW{i}, CellH{i}] = nmfh_lite(T, H0(1:k, :), 100, 100, 1e-4);
        
        save(['Cells' num2str(frame_i) '_clust.mat'], '-v7.3');
    end
end
