﻿

#pragma checksum "C:\Users\Ir Christian Scott\documents\visual studio 2015\Projects\BookApp\BookApp\BookApp.Windows\HomePage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "537F83669C4FC5CC000C44B212DB4D5A"
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
    partial class HomePage : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 50 "..\..\HomePage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.open_library_Click;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 51 "..\..\HomePage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.open_paid_Click;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 52 "..\..\HomePage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.open_book_Click;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


