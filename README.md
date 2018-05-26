# REnamix (RECONSTRUCTION_Dynamix)
自制分为两部分: Decomposition 和 RECONSTRUCTION.  
**使用前请仔细阅读本文档！**  

下载地址：https://github.com/strongrex2001/REnamix/releases/tag/Renamix.beta.2.1  
(Renamix.beta.2.1.zip，解压后可直接使用)
  
可视化做谱器。并且可以直接导出avi视频，输出文件是renamix_Data下的output.avi文件。  
大量操作均需要由快捷键完成。  

## 已知的bug  
输出的xml文件offset符号反了，需要手动增加/删除数值前负号"-"(已修复但未发布)  
当使用鼠标滚轮滚到0:00前时会导致音频不同步，可以通过进度条将时间滚回0:00后来解决(已修复但未发布)  
  
## 快捷键
Ctrl+Alt+O 打开XML文件  
Ctrl+Alt+I 加载音频  
Ctrl+Alt+R 导出视频  
Ctrl+Alt+K 设置面板  
Ctrl+Alt+S 保存XML文件  
Ctrl+Alt+V 打开/关闭跳转和进度条的面板  
  
Z 鼠标位置创建蓝键
X 鼠标位置创建红键
C 鼠标位置创建黄条
  
G 改变网格方向
  
音符在拖动时会自动在时间上与网格对齐  
Alt+单击 将音符在空间上与网格对齐（鼠标滑过即判定有效）  
Alt+右击 将音符在时间上与网格对齐（鼠标滑过即判定有效）  
  
双击音符打开弹窗 点击弹窗外任意位置关闭弹窗  
弹窗开启时按Alt+1/2/3/4将当前音符Width储存在1/2/3/4长度储存器中  
编辑时按1/2/3/4可切换当前长度储存器【默认为1号储存器】

按住Ctrl上下拖动音符可以改变音符宽度

鼠标滚轮可以进行时间轴滚动

空格 播放/暂停  
左键可以拖动音符（仅单击时刻判定，长按鼠标无效）  
右键可以删除音符（仅右击时刻判定，长按鼠标无效）  
