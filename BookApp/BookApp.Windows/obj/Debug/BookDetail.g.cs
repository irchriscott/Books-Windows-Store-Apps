﻿

#pragma checksum "C:\Users\Ir Christian Scott\documents\visual studio 2015\Projects\BookApp\BookApp\BookApp.Windows\BookDetail.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "21EE77B0D7F14EE20E9F6DB29B4B1522"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BookApp
{
    partial class BookDetail : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 103 "..\..\BookDetail.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.backButton_Click;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 83 "..\..\BookDetail.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.bookReadDescription_Click;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 84 "..\..\BookDetail.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.bookCommand_Click;
                 #line default
                 #line hidden
                break;
            case 4:
                #line 85 "..\..\BookDetail.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.bookRead_Click;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


