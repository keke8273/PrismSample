   M S B A M L   `   `   `  ����K  GQBR.SplashModule, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:QBR.SplashModule.ViewModelsQBR.SplashModule.ViewModels  5   	   )    #QBR.SplashModule.Views.SplashScreen   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   Hmc;http://schemas.openxmlformats.org/markup-compatibility/2006    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   @
viewModels)clr-namespace:QBR.SplashModule.ViewModels     5   	     � Title$  SplashScreen��5   	   $	��300��6   $	��500��6+    � WindowStyle$
 None=�67    � ShowInTaskbar .   � AllowsTransparency . 
������  � WindowStartupLocation$ CenterScreen=�6�    � 	Resources 5	      7   1QBR.SplashModule.ViewModels.SplashScreenViewModel   DesignViewModel   Status��%H   &	          5
   
   $ Downloading file 5K/10K��5   w   5      5      .��5      � 
���iii� � RowDefinitions 5   
   �� 5      $
��0.3* �6   �� 5      $
��0.5* �6   �� 5      $
��0.2* �6    � ColumnDefinitions 5   
   �� 5      $
��0.2* �6   �� 5      $
��0.8* �6   .+�5   
   �� $��0��6   $��0��6     DoubleCogIcon#��[ 6-   $��48��6U   $��48��6`   6n   �� 5   
   $��0��6   $��1��6!   
�������$��31(�6D   $��Center=�6R   QC Batch Release Tool6�   �� 5   
   $��2��6   $��1��6(   
�������$��	30,0,0,30q�6l   $��24(�6   ��65   �� *ApplicationVersion+6�   �� 5   
   $��2��6   $��1��6(   $��Bottom=�65   
�������$��	30,0,0,30q�6c   (&© 2014 Universal Biosensors Pty. Ltd.6�   �� 5   
   $��2��6   $��1��6(   $��Bottom=�65   
�������$��	30,0,0,10q�6c   All rights reserved.6�   �� 5   
   $��2��6   $��2��6!   
���   �6K   �� 5   
   $��3��6   $��2��6!   $��	30,10,0,0q�6M   
��������65   �� *Status+6z   5       5!      