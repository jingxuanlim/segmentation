if os.path.isfile(output_dir + 'brain_mask0.hdf5'):
    try:
        mask_reset = eval(input('Reset brain_mask? [0, no]; 1, yes. '))
    except SyntaxError:
        mask_reset = 0
else:
    mask_reset = 1

if mask_reset:
    for frame_i in range(imageframe_nmbr):
        
        # get image mean
        def get_img_hdf(name_i):
            image_filename = image_dir(name_i, frame_i) + 'image_aligned.hdf5'
            with h5py.File(image_filename, 'r') as file_handle:
                return file_handle['V3D'][()].T
        
        image_dims = get_img_hdf(image_names[0]).shape
        assert(np.allclose(image_dims, (lx//ds, ly//ds, lz)))
        image_accumulator = \
            sc.accumulator(np.zeros(image_dims, dtype='float32'), accum_param())
        try:
            sc.parallelize(image_names).foreach(
                lambda name_i: image_accumulator.add(get_img_hdf(name_i)))
        except:
            print('Image-mean parallelization failed -- proceeding serially.')
            for name_i in image_names:
                image_accumulator.add(get_img_hdf(name_i))
        
        image_mean = 1.0 * image_accumulator.value / lt
        
        # get medium and fine resolution peaks
        def medin_filt(img, ftp):
            return ndimage.filters.median_filter(img, footprint=ftp)

        image_peak      = image_mean > medin_filt(image_mean, cell_ball)
        image_peak_fine = image_mean > medin_filt(image_mean, cell_ball_fine)

        # get and save brain mask
        mask_flag = 0
        while mask_flag == 0:

            def knee_plot(image_array, xlabel, ylabel, default_val):
                plt.figure(1, (81, 4))
                thr_range = np.linspace(image_array.min(), image_array.max(), 1000)
                n_suprathr_voxs = np.array([np.mean(image_array > thr) for thr in thr_range])
                plt.plot(thr_range, n_suprathr_voxs)
                plt.xlabel(xlabel); plt.xlim(np.percentile(thr_range, (40, 60))), plt.xticks(thr_range[::100]);
                plt.ylabel(ylabel); plt.ylim([0, 1])
                plt.show()
                try:
                    return eval(input('Enter threshold for: ' + xlabel + ' [default ' + str(default_val) + ']: '))
                except SyntaxError:
                    return default_val

            if thr_mask:
                mask_flag = 1
            else:
                thr_mask = knee_plot(
                    image_array=image_mean,
                    xlabel='Individual pixels -- mean signal over time points',
                    ylabel='Fraction of pixels',
                    default_val=105)

            # remove all disconnected components less than 5000 cubic microliters in size
            small_obj = int(np.round(5000 * (resn_x * ds * resn_y * ds * resn_z)))
            brain_mask = (image_mean > thr_mask)
            brain_mask = morphology.remove_small_objects(brain_mask, small_obj)
            for i in range(lz):
                plt.figure(1, (12, 6))
                plt.subplot(121); plt.imshow((image_mean * (    brain_mask))[:, :, i].T, cmap='hot')
                plt.subplot(122); plt.imshow((image_peak * (1 + brain_mask))[:, :, i].T, cmap='hot')
                plt.show()

            if not mask_flag:
                try:
                    mask_flag = eval(input( 'Is thr_mask = ' + str(thr_mask) + \
                                        ' accurate? [1, yes]; 0, no. '))
                except SyntaxError:
                    mask_flag = 1
        
        plt.close('all')
        
        with h5py.File(output_dir + 'brain_mask' + str(frame_i) + '.hdf5', 'w') as file_handle:
            file_handle['brain_mask']      = brain_mask.T
            file_handle['image_mean']      = image_mean.T
            file_handle['image_peak']      = image_peak.T
            file_handle['image_peak_fine'] = image_peak_fine.T
            file_handle['thr_mask']        = thr_mask
