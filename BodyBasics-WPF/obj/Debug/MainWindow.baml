   M S B A M L   `   `   `  ����  BodyBasics-WPF4    .Microsoft.Samples.Kinect.BodyBasics.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  Body Basics��5   	   $	��600��5   	   $	��700��6    � 	Resources 5         MediumGreyBrush �� Color  KinectPurpleBrush  KinectBlueBrush��%�   &	        &	 #     &	 F     �� 5	   
   $ 	#ff6e6e6e��62   �� 5
   
   $ 	#ff52318f��64   �� 5   
   $ 	#ff00BCF2��62   5      .��5      � $��	10 0 10 0q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   $��0 0 -1 0q�6!   $��Right=�63   $��Bottom=�6O   #��[  6j   $��Segoe UI)�6�   $��18(�6�   �� 6�    � Language$ en-gb�6�   $�Throwing motion study��6�   6�   �� 5   
   $��0��6   $��Images\Status.png��6   $
��None=�68   $
��Left=�6G   $��0,5,0,0q�6b    J� RenderTransformOrigin$ -0.113,0.584&�6s   (� 5   
   $��1��6   $��Center=�6   .��5      �� $��UniformToFill=�63   ��6   �� *ImageSource+63   5      �� 5   
   -   5        $��	statusBar��5   ?   $��2��6   $��Stretch=�6!   $��Bottom=�6P   
�������#��[  6~   .@�5      �� ��6   �� *
StatusText+5      �� 5   
   -   5         ��Name$ RecordCheckBox��5      $��Record��6+   $
��Left=�6<   $��	94,19,0,0q�6W   $	��Top=�6j   6�   �� 5   
   -   5         ��Name$ FileNameBox��5      $
��Left=�6'   $��23��6B   $��
185,17,0,0q�6N    �� TextWrapping$
 Wrap=�6b   $	��Top=�6v   $	��120��6�   6�   �� 5   
   -   5        	 ��Name$	 
SaveButton��5      $
��Save��6%   $
��Left=�64   $��
305,19,0,0q�6O   $	��Top=�6c   $��75��6{   6�   �� 5    
   -   5        $	 ResetButton��5       $��Reset��6&   $
��Left=�66   $��
392,19,0,0q�6Q   $	��Top=�6e   $��75��6}   $ -0.076,0.433&�6�   6�   5!      5"      