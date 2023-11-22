import cv2
import numpy as np

# 读取txt文件
data = {'x': [], 'y': [], 't': []}
with open('整合：高水平组.txt', 'r') as file:
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

# 转换数据集的坐标
opencv_dataset = []
for i in range(len(data['x'])):
    x = data['x'][i]
    y = (height - data['y'][i])# 取反并减去图像高度
    opencv_dataset.append((x, y))


# 初始化视频捕获对象
cap = cv2.VideoCapture('capture_2023-10-09_19-02-03_640x480.mp4')
color_map = cv2.COLORMAP_JET
# 创建窗口
cv2.namedWindow('Output Video', cv2.WINDOW_NORMAL)
cv2.resizeWindow('Output Video', 640, 480)



while True:
    # 逐帧读取视频
    ret, frame = cap.read()

    if not ret:
        break
    
    # # 转为HSV颜色空间
    # hsv = cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)

    # 定义空白图像
    heatmap = np.zeros((frame.shape[0], frame.shape[1]), dtype=np.uint8)

    # 遍历data字典中的每个点并在相应位置添加热力值
    for i in range(len(data['x'])):
        x, y = opencv_dataset[i]
        t = data['t'][i]  # 时间戳，注意是秒
        # 计算当前帧时间与该点时间差
        diff = abs(cap.get(cv2.CAP_PROP_POS_MSEC) - t)
        # 根据时间差计算权重
        weight = max(1 - (diff / 1000), 0)  # 假设1秒内距离最远的点权重为0
        cv2.circle(heatmap, (x, y), radius=5, color=int(255 * weight), thickness=-1)

    # 应用热力图颜色映射并将其覆盖到原始视频帧上
    heatmap = cv2.applyColorMap(heatmap, color_map)
    result = cv2.addWeighted(frame, 0.7, heatmap, 0.3, 0)

    # 显示输出结果
    cv2.imshow('Output Video', result)
    
    # 按q键退出
    if cv2.waitKey(1) == ord('q'):
        break
    
# print(data)   
print(opencv_dataset)
# 释放资源并关闭窗口
cap.release()
cv2.destroyAllWindows()