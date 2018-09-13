<<<<<<< HEAD
def show_vol(IDX, Cell_X, Cell_Y, Cell_Z, x, y, z, Cell_spcesers, Cell_bandpowr, Cell_signalpr):
    SNV = [[],[]]
    for h in range(2):
        SNV[h] = np.zeros((x, y, z));
        for i in IDX[h]:
            for j in range(np.count_nonzero(np.isfinite(Cell_X[i]))):
                xij, yij, zij = int(Cell_X[i, j]), int(Cell_Y[i, j]), int(Cell_Z[i, j])
                SNV[h][xij, yij, zij] = np.maximum(SNV[h][xij, yij, zij], Cell_spcesers[i, j])
                
    cmax = np.percentile(SNV[0], 99)
    
    pp.figure(1, (12, 4))
    pp.subplot(121);
    _ = pp.hist(Cell_bandpowr[IDX[0]], 100)
    pp.title('Component timeseries power histogram')
    pp.subplot(122);
    _ = pp.hist(Cell_signalpr[IDX[0]], 100)
    pp.title('Signal vs noise probability threshold')
    pp.show()
    
    for i in range(z):
        pp.figure(1, (12, 4))
        pp.subplot(121), pp.imshow(SNV[0][:,:,i].T, vmin=0, vmax=cmax, cmap='hot')
        pp.title('%d cells to be kept.' %IDX[0].shape)
        pp.subplot(122), pp.imshow(SNV[1][:,:,i].T, vmin=0, vmax=cmax, cmap='hot')
        pp.title('%d cells to be deleted.' %IDX[1].shape)
        pp.show()
    
    pp.figure(1, (12, 4))
    pp.subplot(121), pp.imshow(SNV[0].max(2).T, vmin=0, vmax=cmax, cmap='hot')
    pp.title('%d cells to be kept (maximum projection).' %IDX[0].shape)
    pp.subplot(122), pp.imshow(SNV[1].max(2).T, vmin=0, vmax=cmax, cmap='hot')
    pp.title('%d cells to be deleted (maximum projection).' %IDX[0].shape)
    pp.show()
    
    
for frame_i in range(imageframe_nmbr):

=======
<<<<<<< HEAD
for frame_i in range(imageframe_nmbr):

    thr_signalprob_default = 0.5
=======
def show_vol(IDX, Cell_X, Cell_Y, Cell_Z, x, y, z, Cell_spcesers, Cell_bandpowr, Cell_signalpr):
    SNV = [[],[]]
    for h in range(2):
        SNV[h] = np.zeros((x, y, z));
        for i in IDX[h]:
            for j in range(np.count_nonzero(np.isfinite(Cell_X[i]))):
                xij, yij, zij = int(Cell_X[i, j]), int(Cell_Y[i, j]), int(Cell_Z[i, j])
                SNV[h][xij, yij, zij] = np.maximum(SNV[h][xij, yij, zij], Cell_spcesers[i, j])
                
    cmax = np.percentile(SNV[0], 99)
    
    pp.figure(1, (12, 4))
    pp.subplot(121);
    _ = pp.hist(Cell_bandpowr[IDX[0]], 100)
    pp.title('Component timeseries power histogram')
    pp.subplot(122);
    _ = pp.hist(Cell_signalpr[IDX[0]], 100)
    pp.title('Signal vs noise probability threshold')
    pp.show()
    
    for i in range(z):
        pp.figure(1, (12, 4))
        pp.subplot(121), pp.imshow(SNV[0][:,:,i].T, vmin=0, vmax=cmax, cmap='hot')
        pp.title('%d cells to be kept.' %IDX[0].shape)
        pp.subplot(122), pp.imshow(SNV[1][:,:,i].T, vmin=0, vmax=cmax, cmap='hot')
        pp.title('%d cells to be deleted.' %IDX[1].shape)
        pp.show()
    
    pp.figure(1, (12, 4))
    pp.subplot(121), pp.imshow(SNV[0].max(2).T, vmin=0, vmax=cmax, cmap='hot')
    pp.title('%d cells to be kept (maximum projection).' %IDX[0].shape)
    pp.subplot(122), pp.imshow(SNV[1].max(2).T, vmin=0, vmax=cmax, cmap='hot')
    pp.title('%d cells to be deleted (maximum projection).' %IDX[0].shape)
    pp.show()
    
    
for frame_i in range(imageframe_nmbr):

>>>>>>> 879ea3f81bd418d5a22b253fdc2becf2e337c3c2
>>>>>>> 188dfe11ca6e1034686919ab53100f8050f02720
    thr_similarity = 0.5
    freq_lims = (0.01, 0.1)

    with h5py.File(output_dir + 'Cells' + str(frame_i) + '.hdf5', 'r') as file_handle:
<<<<<<< HEAD
        Cmpn_position = file_handle['Cell_position'][()]
        Cmpn_spcesers = file_handle['Cell_spcesers'][()]
        Cmpn_timesers = file_handle['Cell_timesers'][()]
        x, y, z, t = file_handle['dims'][()]
=======
<<<<<<< HEAD
        Cell_position = file_handle['Cell_position'][()]
        Cell_spcesers = file_handle['Cell_spcesers'][()]
        Cell_timesers = file_handle['Cell_timesers'][()]
>>>>>>> 188dfe11ca6e1034686919ab53100f8050f02720
        freq = file_handle['freq'][()]
        
    Cmpn_X = Cmpn_position[:, :, 0]
    Cmpn_Y = Cmpn_position[:, :, 1]
    Cmpn_Z = Cmpn_position[:, :, 2]

    if ((t > 2e4) and (freq < 10)) or (~np.isfinite(freq)):
        while 1:
            try:
                freq = eval(input('t: %d, freq: %.3f. Enter frequency: ' %(t, freq)))
                print('Continuing with frequency: %f' %freq)
                break
            except SyntaxError:
                pass
    
    ix = np.any(np.isnan(Cmpn_timesers), 1);
    if np.any(ix):
        print('nans: %d' %np.sum(ix))
        Cmpn_timesers[ix] = np.min(Cmpn_timesers[np.where(Cmpn_timesers)]);

    freq, Cmpn_pwsd = signal.periodogram(Cmpn_timesers, 1000 / t_stack, axis=1)

    lidx0 = (freq > freq_lims[0]) & (freq < freq_lims[1])
<<<<<<< HEAD
=======
    Cell_band = np.log10(np.sum(Cell_pwsd[:, lidx0], 1))[:, None]
    gmm = mixture.GMM(n_components=2, n_iter=100, n_init=100).fit(Cell_band)
    Cell_signalpr = gmm.predict_proba(Cell_band)
    Cell_signalpr = Cell_signalpr[:, np.argmax(Cell_band[np.argmax(Cell_signalpr, 0)])]
=======
        Cmpn_position = file_handle['Cell_position'][()]
        Cmpn_spcesers = file_handle['Cell_spcesers'][()]
        Cmpn_timesers = file_handle['Cell_timesers'][()]
        x, y, z, t = file_handle['dims'][()]
        freq = file_handle['freq'][()]
        
    Cmpn_X = Cmpn_position[:, :, 0]
    Cmpn_Y = Cmpn_position[:, :, 1]
    Cmpn_Z = Cmpn_position[:, :, 2]

    if ((t > 2e4) and (freq < 10)) or (~np.isfinite(freq)):
        while 1:
            try:
                freq = eval(input('t: %d, freq: %.3f. Enter frequency: ' %(t, freq)))
                print('Continuing with frequency: %f' %freq)
                break
            except SyntaxError:
                pass
    
    ix = np.any(np.isnan(Cmpn_timesers), 1);
    if np.any(ix):
        print('nans: %d' %np.sum(ix))
        Cmpn_timesers[ix] = np.min(Cmpn_timesers[np.where(Cmpn_timesers)]);

    freq, Cmpn_pwsd = signal.periodogram(Cmpn_timesers, 1000 / t_stack, axis=1)

    lidx0 = (freq > freq_lims[0]) & (freq < freq_lims[1])
>>>>>>> 188dfe11ca6e1034686919ab53100f8050f02720
    Cmpn_bandpowr = np.log10(np.sum(Cmpn_pwsd[:, lidx0], 1))[:, None]
    gmm = mixture.GaussianMixture(n_components=2, max_iter=100, n_init=100).fit(Cmpn_bandpowr)
    Cmpn_signalpr = gmm.predict_proba(Cmpn_bandpowr)
    Cmpn_signalpr = Cmpn_signalpr[:, np.argmax(Cmpn_bandpowr[np.argmax(Cmpn_signalpr, 0)])]
<<<<<<< HEAD
=======
>>>>>>> 879ea3f81bd418d5a22b253fdc2becf2e337c3c2
>>>>>>> 188dfe11ca6e1034686919ab53100f8050f02720

    thrs_flag = 0
    while thrs_flag == 0:
        pp.figure(1, (12, 4))
        pp.subplot(121);
<<<<<<< HEAD
        _ = pp.hist(Cmpn_bandpowr, 100);
        pp.title('Component timeseries power histogram')

=======
<<<<<<< HEAD
        _ = pp.hist(Cell_band, 100);
        xlim1 = pp.xlim()
>>>>>>> 188dfe11ca6e1034686919ab53100f8050f02720
        pp.subplot(122);
        _ = pp.hist(Cmpn_signalpr, 100)
        pp.title('Signal vs noise probability threshold')
        pp.show()

        try:
            thr_prob = eval(input('Enter signal probability threshold [default 0.5]: '))
        except SyntaxError:
            thr_prob = 0.5

        ix = np.argmin(np.abs(Cmpn_signalpr - thr_prob))
        thr_powr = Cmpn_bandpowr[ix]
        if np.isinf(thr_prob):
            thr_powr = thr_prob
        
        Cell_validity = (Cmpn_signalpr > thr_prob) & (Cmpn_bandpowr.ravel() > thr_powr)
        
        IDX = [np.where(Cell_validity)[0], np.where(~Cell_validity)[0]]
        
        show_vol(IDX, Cmpn_X, Cmpn_Y, Cmpn_Z, x, y, z, Cmpn_spcesers, Cmpn_bandpowr, Cmpn_signalpr)

        try:
<<<<<<< HEAD
            thrs_flag = eval(input('Is thr_prob=' + str(thr_prob) + ' accurate? [1, yes]; 0, no. '))
=======
            thrs_flag = input('Is thr_signalprob=' + str(thr_signalprob) + ' accurate? [1, yes]; 0, no. ')
=======
        _ = pp.hist(Cmpn_bandpowr, 100);
        pp.title('Component timeseries power histogram')

        pp.subplot(122);
        _ = pp.hist(Cmpn_signalpr, 100)
        pp.title('Signal vs noise probability threshold')
        pp.show()

        try:
            thr_prob = eval(input('Enter signal probability threshold [default 0.5]: '))
        except SyntaxError:
            thr_prob = 0.5

        ix = np.argmin(np.abs(Cmpn_signalpr - thr_prob))
        thr_powr = Cmpn_bandpowr[ix]
        if np.isinf(thr_prob):
            thr_powr = thr_prob
        
        Cell_validity = (Cmpn_signalpr > thr_prob) & (Cmpn_bandpowr.ravel() > thr_powr)
        
        IDX = [np.where(Cell_validity)[0], np.where(~Cell_validity)[0]]
        
        show_vol(IDX, Cmpn_X, Cmpn_Y, Cmpn_Z, x, y, z, Cmpn_spcesers, Cmpn_bandpowr, Cmpn_signalpr)

        try:
            thrs_flag = eval(input('Is thr_prob=' + str(thr_prob) + ' accurate? [1, yes]; 0, no. '))
>>>>>>> 879ea3f81bd418d5a22b253fdc2becf2e337c3c2
>>>>>>> 188dfe11ca6e1034686919ab53100f8050f02720
        except SyntaxError:
            thrs_flag = 1

    # brain mask array
<<<<<<< HEAD
    L = [[[[] for zi in range(z)] for yi in range(y)] for xi in range(x)]
    for i in range(len(Cmpn_X)):
=======
<<<<<<< HEAD
    L = [[[[] for zi in xrange(lz)] for yi in xrange(ly//ds)] for xi in xrange(lx//ds)]
    for i in xrange(cn):
>>>>>>> 188dfe11ca6e1034686919ab53100f8050f02720
        if Cell_validity[i]:
            for j in range(np.count_nonzero(np.isfinite(Cmpn_X[i]))):
                xij, yij, zij = int(Cmpn_X[i, j]), int(Cmpn_Y[i, j]), int(Cmpn_Z[i, j])
                L[xij][yij][zij].append(i)

    def get_pairs(xyz, l):
        return [[xyz[i], xyz[j]] for i in range(l) for j in range(i + 1, l)]

    vox_pairs = [get_pairs(xyz, len(xyz)) for X in L for Y in X for xyz in Y if len(xyz) > 1]
    all_pairs = np.sort([p for pairs in vox_pairs for p in pairs], 1)
    _, idx_uolap_pairs, size_uolap_pairs = \
            np.unique(all_pairs[:, 0] + 1j * all_pairs[:, 1], return_index=True, return_counts=True)
    
    uolap_pairs = all_pairs[idx_uolap_pairs]
    size_Cmpn = np.sum(np.isfinite(Cmpn_spcesers), 1)
    spatial_similarity_uolap_pairs = size_uolap_pairs / size_Cmpn[uolap_pairs].mean(1)

    hiolap_pairs = uolap_pairs[spatial_similarity_uolap_pairs > thr_similarity]
    corr_hiolap_pairs = np.zeros((len(hiolap_pairs), 1))
    corr_hiolap_pairs = np.r_[[np.corrcoef(Cmpn_timesers[pi])[0][1] for pi in hiolap_pairs]]
            
    duplicate_pairs = hiolap_pairs[corr_hiolap_pairs > thr_similarity]
    
    Cmpn_wsum = np.nansum(Cmpn_spcesers, 1)
    for i, pi in enumerate(duplicate_pairs):
        Cell_validity[pi[np.argmin(Cmpn_wsum[pi])]] = 0
        
    n = np.count_nonzero(Cell_validity);
    
    Cell_X = Cmpn_X[Cell_validity]
    Cell_Y = Cmpn_Y[Cell_validity]
    Cell_Z = Cmpn_Z[Cell_validity]
    
    Cell_spcesers = Cmpn_spcesers[Cell_validity]
    Cell_timesers0 = Cmpn_timesers[Cell_validity]
    
    Cell_timesers1, Cell_baseline1 = \
<<<<<<< HEAD
=======
        list(zip(*sc.parallelize(Cell_timesers).map(detrend_dynamic_baseline).collect()))

    with h5py.File(output_dir + 'Cells' + str(frame_i) + '.hdf5', 'r+') as filename:
        for i, cell_idx in enumerate(np.nonzero(Cell_validity)[0]):
            hdf5_dir = '/Cell/' + str(i).zfill(6)
            filename[hdf5_dir + '/Cell_position1'] = Cell_position[cell_idx]
            filename[hdf5_dir + '/Cell_spcesers1'] = Cell_spcesers[cell_idx]
            filename[hdf5_dir + '/Cell_timesers1'] = Cell_timesers1[cell_idx]
            filename[hdf5_dir + '/Cell_baseline1'] = Cell_baseline1[cell_idx]
=======
    L = [[[[] for zi in range(z)] for yi in range(y)] for xi in range(x)]
    for i in range(len(Cmpn_X)):
        if Cell_validity[i]:
            for j in range(np.count_nonzero(np.isfinite(Cmpn_X[i]))):
                xij, yij, zij = int(Cmpn_X[i, j]), int(Cmpn_Y[i, j]), int(Cmpn_Z[i, j])
                L[xij][yij][zij].append(i)

    def get_pairs(xyz, l):
        return [[xyz[i], xyz[j]] for i in range(l) for j in range(i + 1, l)]

    vox_pairs = [get_pairs(xyz, len(xyz)) for X in L for Y in X for xyz in Y if len(xyz) > 1]
    all_pairs = np.sort([p for pairs in vox_pairs for p in pairs], 1)
    _, idx_uolap_pairs, size_uolap_pairs = \
            np.unique(all_pairs[:, 0] + 1j * all_pairs[:, 1], return_index=True, return_counts=True)
    
    uolap_pairs = all_pairs[idx_uolap_pairs]
    size_Cmpn = np.sum(np.isfinite(Cmpn_spcesers), 1)
    spatial_similarity_uolap_pairs = size_uolap_pairs / size_Cmpn[uolap_pairs].mean(1)

    hiolap_pairs = uolap_pairs[spatial_similarity_uolap_pairs > thr_similarity]
    corr_hiolap_pairs = np.zeros((len(hiolap_pairs), 1))
    corr_hiolap_pairs = np.r_[[np.corrcoef(Cmpn_timesers[pi])[0][1] for pi in hiolap_pairs]]
            
    duplicate_pairs = hiolap_pairs[corr_hiolap_pairs > thr_similarity]
    
    Cmpn_wsum = np.nansum(Cmpn_spcesers, 1)
    for i, pi in enumerate(duplicate_pairs):
        Cell_validity[pi[np.argmin(Cmpn_wsum[pi])]] = 0
        
    n = np.count_nonzero(Cell_validity);
    
    Cell_X = Cmpn_X[Cell_validity]
    Cell_Y = Cmpn_Y[Cell_validity]
    Cell_Z = Cmpn_Z[Cell_validity]
    
    Cell_spcesers = Cmpn_spcesers[Cell_validity]
    Cell_timesers0 = Cmpn_timesers[Cell_validity]
    
    Cell_timesers1, Cell_baseline1 = \
>>>>>>> 188dfe11ca6e1034686919ab53100f8050f02720
        list(zip(*sc.parallelize(Cell_timesers0).map(detrend_dynamic_baseline).collect()))
    
    Volume = np.zeros((x, y, z))
    Labels = np.zeros((x, y, z))
    for i in range(len(Cell_X)):
        for j in range(np.count_nonzero(np.isfinite(Cell_X[i]))):
            xij, yij, zij = int(Cell_X[i, j]), int(Cell_Y[i, j]), int(Cell_Z[i, j])
            if Cell_spcesers[i, j] > Volume[xij, yij, zij]:
                Volume[xij, yij, zij] = Cell_spcesers[i, j]
                Labels[xij, yij, zij] = i;
                
    with h5py.File(output_dir + 'Cells' + str(frame_i) + '_clean' + '.hdf5', 'w-') as file_handle:
        file_handle['n'] = n
        file_handle['x'] = x
        file_handle['y'] = y
        file_handle['z'] = z
        file_handle['freq'] = freq
        file_handle['Cell_X'] = Cell_X
        file_handle['Cell_Y'] = Cell_Y
        file_handle['Cell_Z'] = Cell_Z
        file_handle['Volume'] = Volume
        file_handle['Labels'] = Labels
        file_handle['Cell_spcesers'] = Cell_spcesers
        file_handle['Cell_timesers0'] = Cell_timesers0
        file_handle['Cell_timesers1'] = Cell_timesers1
        file_handle['Cell_baseline1'] = Cell_baseline1
        
<<<<<<< HEAD
=======
>>>>>>> 879ea3f81bd418d5a22b253fdc2becf2e337c3c2
>>>>>>> 188dfe11ca6e1034686919ab53100f8050f02720
