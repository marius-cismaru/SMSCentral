   M S B A M L   `   `   `  ����E  ASMSCentral, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:SMSCentral
SMSCentral  5   	   !    SMSCentral.Views.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   *localclr-namespace:SMSCentral     5   	   I EFilterDataGrid, Version=1.2.6.0, Culture=neutral, PublicKeyToken=null4control&http://filterdatagrid.control.com/2021  5   	     � Title$  
MainWindow��5	   	   $	��820��6   $
��1360��6)   
�������$��16(�6I    � WindowStartupLocation$ CenterScreen=�5
   	   .��5      �  � RowDefinitions 5   
   �� 5      $	��186 �6   �� 5      $	��206 �6   �� 5       � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $	��400 �6   .+�5   
   ]� $��0��6   $��3��6!   $��0��62   $��0,0,4,0q�6B   .+�5      ]� $	��Top=�6   $��42��6-   
���� ��.+�5      ��  �� Orientation$ 
Horizontal=�6   $
��Left=�66   $
��Left=�6L   .+�5      �� $��10,6,6,6q�5      $��4q�5      $	��100��5      $��Import��5       � ToolTip$  Import receivers from file��5      ��5      �� *ImportReceiversCommand+�� 5      $��6q�5       $��4q�5!      $	��100��5"      $��Delete��5$      $ Delete selected receivers��5%      {�5#      ��  �� ElementName$ FilterDataGrid�� �� Path$ SelectedItems.Count���5&      �� $ FilterDataGrid��$ SelectedItems���5'      �� *DeleteReceiversCommand+5(      �� 5)      $ 
Horizontal=�6   $��Right=�66   $��Right=�6M   .+�5*      �� $��6,6,10,6q�5+      $��4q�5,      $	��100��5-      $��Refresh operators��5/      $. (Refresh operators for selected receivers��50      {�5.      �� $ FilterDataGrid��$ SelectedItems.Count���51      �� $ FilterDataGrid��$ SelectedItems���52      �� *" RefreshOperatorsReceiversCommand+53      54      ]� 55      $��42��6   $	��Top=�6$   .+�56      �� $ 
Horizontal=�6   $
��Left=�66   $��6,0,10,0q�6L   .+�57      �� $��Center=�58      $+��%Search by name, group or phone number��59      $
��Bold"�5:      �� 5;      $	��250��5<      $��Center=�5=      ��5>      �� *Search+ �� UpdateSourceTrigger$ PropertyChanged=�5?      �� 5@      $ 
Horizontal=�6   $��Right=�66   $��Right=�6M   .+�5A      �� $��6,6,10,6q�5B      $��4q�5C      $��Reset filters��5D      ��5E      �� *ResetFiltersReceiversCommand+5F      5G      #  FilterDataGrid.FilterDataGrid  5H      -   5        	  Name$	 FilterDataGrid��5I      &   System.Windows.Controls.DataGrid
   AutoGenerateColumns
 .  $��10q�5K         DateFormatString$ d��5L         FilterLanguage$ English=�5M         SelectionMode$ Extended=�5P         SelectionUnit$ FullRow=�5Q         ShowElapsedTime .    ShowRowsCount .    ShowStatusBar .    FilterDatagridStyle#��[  5U      ��5N      �� *	Receivers+$ PropertyChanged=� �� SelectedItem 5O      �� *ReceiverSelected+ �� Mode$ OneWayToSource=�   Columns 5V      +  %FilterDataGrid.DataGridTemplateColumn  5X         	FieldName$ PhoneNumber��5Y      ,  &System.Windows.Controls.DataGridColumn   Header$ Phone number��5Z         Width5  /System.Windows.Controls.DataGridLengthConverter$
 auto 5[         IsColumnFiltered .    SortMemberPath$ PhoneNumber��5]      4  .System.Windows.Controls.DataGridTemplateColumn   CellTemplate 5^      �� 5_       �� DataType$ local:Receiver��6+   .R�5`   "   ��  �� Padding$	 2,0q�6,   ��6:   �� *PhoneNumber+5a      5c      '  !FilterDataGrid.DataGridTextColumn  5e      $
 Name��5g      $
 auto 5h         IsColumnFiltered . 1  +System.Windows.Controls.DataGridBoundColumn   Binding 5f      �� *Name+5i        5k      $ Group��5m      $
 auto 5n       .  5l      �� *Group+5o        5q      $ Operator��5s      $
 auto 5t       .  5r      �� *OperatorDescription+5u        5w      $ DateRefreshOperator��5x      $ Date refresh operator��5y      $
 auto 5z       . $ DateRefreshOperator��5|       5}      �� 5~      $ local:Receiver��6+   .R�5   "   �� $	 2,0q�6,   ��6:   �� * DateRefreshOperatorDescription+5�      5�        5�      $ DateSendSMS��5�      $ Date send last SMS��5�      $
 auto 5�       . $ DateSendSMS��5�       5�      �� 5�      $ local:Receiver��6+   .R�5�   "   �� $	 2,0q�6,   ��6:   �� *DateSendSMSDescription+5�      5�      5�      5�      �� 5�   
   $��0��5�      $��Right=�5�      $��Stretch=�5�      $��3��5�      
�������$��4��5�      ]� 5�   
   $��0��6   $��1��6!   .+�5�      ]� $	��Top=�6   $��42��6-   
���� ��.+�5�      �� $ 
Horizontal=�6   $
��Left=�66   $
��Left=�6L   .+�5�      �� $��0,0,10,0q�5�      $��Center=�5�      $��Receiver��5�      
�������$
��Bold"�5�      5�      �� 5�      $ 
Horizontal=�6   $��Right=�66   $��Right=�6M   .+�5�      �� $��6,6,10,6q�5�      $��4q�5�      $	��100��5�      $
��Save��5�      $ Save receiver��5�      ��5�      �� *SaveReceiverCommand+5�      5�      �� 5�      $ Vertical=�6   $��0,6,0,0q�60   .+�5�      ]� .+�5�      �� $
��Left=�5�      $��Center=�5�      $��Right=�5�      $��Phone number��5�      $	��120��5�      �� 5�      $��0,0,10,0q�5�      $��Center=�5�      ��5�      �� *ReceiverUpdated.PhoneNumber+5�      ]� 5�      .+�5�      �� $
��Left=�5�      $��Center=�5�      $��Right=�5�      $
��Name��5�      $	��120��5�      �� 5�      $��0,0,10,0q�5�      $��Center=�5�      ��5�      �� *ReceiverUpdated.Name+5�      ]� 5�      .+�5�      �� $
��Left=�5�      $��Center=�5�      $��Right=�5�      $��Group��5�      $	��120��5�      �� 5�      $��0,0,10,0q�5�      $��Center=�5�      ��5�      �� *ReceiverUpdated.Group+5�      ]� 5�      .+�5�      �� $
��Left=�5�      $��Center=�5�      $��Right=�5�      $��Operator��5�      $	��120��5�      �� 5�      $��0,0,10,0q�5�      $��Center=�5�      ��5�      �� *	Operators+$ PropertyChanged=� 5�      �� *ReceiverUpdated.Operator+5�      5�      5�      ]� 5�   
   $��1��6   $��1��6!   .+�5�      ]� $	��Top=�6   $��42��6-   
���� ��.+�5�      �� $ 
Horizontal=�6   $
��Left=�66   $
��Left=�6L   .+�5�      �� $��0,0,10,0q�5�      $��Center=�5�      $	��SMS��5�      
�������$
��Bold"�5�      5�      �� 5�      $ 
Horizontal=�6   $��Right=�66   $��Right=�6M   .+�5�      �� $��6,6,10,6q�5�      $��4q�5�      $	��100��5�      $
��Send��5�      $$ Send SMS to selected receivers��5�      {�5�      �� $ FilterDataGrid��$ SelectedItems.Count���5�      �� $ FilterDataGrid��$ SelectedItems���5�      �� *SendSMSReceiversCommand+5�      5�      �� 5�      $ Vertical=�6   $��10q�60   .+�5�      ��  �� TextWrapping$
 Wrap=�5�        �� AcceptsReturn  . $! �� HorizontalScrollBarVisibility$! Disabled=�5�      "" �� VerticalScrollBarVisibility$
" Auto=�5�      $	��144��5�   V   ��6   �� *
SMSMessage+$ PropertyChanged=�6V   5      5     ]� 5  
   $��2��6   $��1��6!   .+�5     ]� $	��Top=�6   $��42��6-   
���� ��.+�5     �� $ 
Horizontal=�6   $
��Left=�66   $
��Left=�6L   .+�5     �� $��0,0,10,0q�5     $��Center=�5     $��Modems��5     
�������$
��Bold"�5
     5     �� 5     $ 
Horizontal=�6   $��Right=�66   $��Right=�6M   .+�5     �� $��6,6,10,6q�5     $��4q�5     $	��100��5     $��Refresh��5     $ Refresh modems��5     ��5     �� *RefreshModemsCommand+5     5       5     $ Single=�6   
 .  #   CanUserAddRows# .  $��10q�6�   ��6J   �� *Modems+$ PropertyChanged=�$   RowStyle$ 5     �� 5     % �� 
TargetType)	  #System.Windows.Controls.DataGridRow"% 	 6   .�5     �� & �� Property& �@J�IsHitTestVisible' �� Value' .  6M   ( �� Triggers( 5     �� 5     ) �� Binding) 6*   �� *Status+* �� Value
* Failed6E   .a�5  "   �� & � ��' �#ff85665  #   5     �� 5     ) 6*   �� *Status+* OK6E   .a�5   "   �� & � ��' �#c2f0c25!  #   5"     �� 5#     ) 6*   �� *Status+* N/A6E   .a�5$  "   �� & � ��
' �����5%  #   5&     5(      5+     0
  *System.Windows.Controls.DataGridTextColumn
  5,     $
 auto 6)   $ Status��66    6F   �� *Status+6c   
  5-     $
 auto 6)   $ Operator��66    6H   �� *OperatorDescription+6r   
  5.     $
 auto 6)   $ Number��66    6F   �� *PhoneNumber+6h   
  5/     $
 auto 6)   $ Priority��66    6H   �� *
Priority+6g   
  50     $
 auto 6)   $
 Name��66    6D   �� *Name+6_   
  51     $
 auto 6)   $ Com Port��66    6H   �� *	COMPort+6f   53     54     55     56     