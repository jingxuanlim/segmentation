# -*- coding: utf-8 -*-

# clean all previous matplotlib plots
plt.close("all")
clear_all()

import imreg

fname=r'D:\Takashi\Jing\2P\noradrenergic_00.tif'

# read image files
stack=imread(fname)

# plt.imshow(stack[1,:,:].squeeze())

# register images to reduce drift -- as many times as deemed necessary
stack_registered1=imreg.stackRegister_simple(stack, stack.mean(axis=0).squeeze())
imsave(r'D:\Takashi\Jing\2P\noradrenergic_00_registerd1.tif',stack_registered1[0].astype('uint16'))

stack_registered2=imreg.stackRegister_simple(stack_registered1[0], stack_registered1[0].mean(axis=0).squeeze())
imsave(r'D:\Takashi\Jing\2P\noradrenergic_00_registerd2.tif',stack_registered2[0].astype('uint16'))
