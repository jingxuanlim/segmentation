clear variables
output_dir = [pwd '/'];
cd(output_dir)

%%

poly_ordr = 2;          % 1;            % 2;
band_filt = 0;          % 0;            % 1;
offs_secn = [60 0];     % [60 0];       % [60 60];

f_lo = 0.001;
tau_p = 60e3;           % 600e3;        % 300e3;
smt_f = 0.25;           % 0.5;          % 0.25;

imageframe_nmbr = h5read([output_dir 'prepro_parameters.hdf5'], '/imageframe_nmbr');
dt_range = 1 + double(h5read([output_dir 'prepro_parameters.hdf5'], '/dt_range'));

%%

for frame_i = 0:imageframe_nmbr-1
    fi = num2str(frame_i);
    %%
    
    if exist([output_dir 'brain_mask' fi '.hdf5'], 'file')
        image_mean = h5read([output_dir 'brain_mask' fi '.hdf5'], '/image_mean');
    elseif exist([output_dir 'image_reference_aligned' fi '.nii.gz'], 'file')
        nii = nii_load([output_dir 'image_reference_aligned' fi '.nii.gz']);
        lpad = h5read([output_dir 'prepro_parameters.hdf5']);
        image_mean = image_mean(:, :, lpad+1:end-lpad);
    end
    
    load(['Cells' fi '_clean.mat'], ...
        'Cell_spcesers', 'Cell_timesers', 'freq', ...
        'Cell_X', 'Cell_Y', 'Cell_Z');
    [n, t] = size(Cell_timesers);
    freq = double(freq);
    
    if ~exist('freq', 'var')
        freq = h5read([output_dir 'prepro_parameters.hdf5'], '/freq_stack');
    end
    offset = round(offs_secn * freq);
    
    l_ar = round(3 * freq / f_lo);
    t_stack = 1000 / freq;
    len_p = round(tau_p / t_stack);
    rho_p = round(0.1 * len_p);
    
    %%
    
    t_idx = false(1, t);
    t_idx(offset(1)+1:end-offset(2)) = 1;
    
    Cell_timesers1 = zeros(size(Cell_timesers));
    Cell_baseline1 = zeros(size(Cell_timesers));
    parfor i = 1:n
        disp(i)
        
        mean0 = 0;
        for j = 1:nnz(isfinite(Cell_spcesers(i, :)))
            mean0 = mean0 + ...
                Cell_spcesers(i, j) * image_mean(Cell_X(i, j), Cell_Y(i, j), Cell_Z(i, j));
        end
        mean0 = mean0 / nansum(Cell_spcesers(i, :));
        
        time0 = mean0 * Cell_timesers(i, :) / mean(Cell_timesers(i, :));
        p0 = polyfit(find(t_idx), time0(t_idx), poly_ordr);
        trnd0 = polyval(p0, 1:t);
        time1 = time0 - trnd0;
        
        if band_filt
            tpadd = zeros(1, round((3*l_ar-t)/2) + 1);
            time1 = bandpass_filter([tpadd time1 tpadd], freq, [f_lo, inf], l_ar);
            time1 = time1(numel(tpadd) + (1:t));
        end
        
        if numel(dt_range) < t
            base0 = ordfilt2(time1(dt_range), rho_p, ones(1, len_p), 'symmetric');
            base0 = interp1(time1(dt_range) + 1e-6*rand(1, numel(dt_range)), base0, time1);
            base0 = smooth(base0, smt_f, 'loess').';
        else
            base0 = ordfilt2(time1, rho_p, ones(1, len_p), 'symmetric');
            base0 = smooth(base0, smt_f, 'loess').';
        end
        
        base1 = base0 + quantile(time1(t_idx) - base0(t_idx), 0.01);
        
        time1(~t_idx) = mean(time1(t_idx));
        base1(~t_idx) = mean(time1(t_idx));
        
        Cell_timesers1(i, :) = time1;
        Cell_baseline1(i, :) = base1;
    end
    clear Cell_timesers freq
    
    save(['Cells' num2str(frame_i) '_baseline.mat'], '-v7.3');
end

delete(gcp('nocreate'))
