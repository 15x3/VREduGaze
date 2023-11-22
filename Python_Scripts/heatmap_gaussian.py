import cv2
import numpy as np
from scipy.ndimage.filters import gaussian_filter

# 读取txt文件
data = {'x': [], 'y': [], 't': []}
with open('整合：低水平组.txt', 'r') as file:
    lines = file.readlines()
    for line in lines:
        values = line.split(',')
        x = int(float(values[0]))
        y = int(float(values[1]))
        t = float(values[3]) * 1000
        data['x'].append(x)
        data['y'].append(y)
        data['t'].append(t)

# OpenCV图像的宽度和高度
width = 640
height = 480

# 读取背景图像
background_image = cv2.imread('background.png')
background_image = cv2.resize(background_image, (width, height))

# 创建空白灰度图像
image = np.zeros((height, width), dtype=np.uint8)

# 绘制坐标点到图像上
for i in range(len(data['x'])):
    x = data['x'][i]
    y = (height - data['y'][i])
    cv2.circle(image, (x, y), 4, 255, -1)

# 计算点的密度
density = gaussian_filter(image.astype(float), sigma=10)

# 将密度映射为颜色值
heatmap = cv2.applyColorMap((density / np.max(density) * 255).astype(np.uint8), cv2.COLORMAP_JET)

# 将热力图叠加到背景图像上
result = cv2.addWeighted(background_image, 0.3, heatmap, 0.7, 0)

# 将灰度图像转换为热力图
# heatmap = cv2.applyColorMap(image, cv2.COLORMAP_JET)

# 显示灰度图像
# cv2.imshow("Gray Image", image)

# 显示热力图
cv2.imshow("Heatmap", result)
cv2.waitKey(0)
cv2.destroyAllWindows()
