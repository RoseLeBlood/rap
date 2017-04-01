/*abgewandelte MIT-Licens 
NON-COMMERCIAL LICENSE
Ramona Audio Player Copyright (c)  2013 Philipp Schröck <philsch@hotmail.de>

1.0  Hiermit wird unentgeltlich, jeder Person, die eine Kopie der Software und der zugehörigen 
Dokumentationen (die \"Software\") erhält, die Erlaubnis erteilt, sie uneingeschränkt zu benutzen, 
inklusive und ohne Ausnahme,  dem Recht, sie zu verwenden, kopieren, ändern und/oder verbreiten und Personen, 
die diese Software erhalten, diese Rechte zu geben, unter den folgenden Bedingungen:

Der obige Urheberrechtsvermerk, dieser Erlaubnisvermerk und der Quellcode sind in allen Kopien oder Teilkopien 
der Software beizulegen.

2.0 Die Kommerzielle Nutzung dieser Software oder Teile aus der Software ist ausdrücklich verboten.

3.0 DIE SOFTWARE WIRD OHNE JEDE AUSDRÜCKLICHE ODER IMPLIZIERTE GARANTIE BEREITGESTELLT, EINSCHLIESSLICH DER 
GARANTIE ZUR BENUTZUNG FÜR DEN VORGESEHENEN ODER EINEM BESTIMMTEN ZWECK SOWIE  
JEGLICHER RECHTSVERLETZUNG, JEDOCH NICHT DARAUF BESCHRÄNKT. IN KEINEM FALL SIND DIE AUTOREN ODER 
COPYRIGHTINHABER FÜR JEGLICHEN SCHADEN ODER SONSTIGE ANSPRÜCHE HAFTBAR ZU MACHEN, OB INFOLGE DER 
ERFÜLLUNG EINES VERTRAGES, EINES DELIKTES ODER ANDERS IM ZUSAMMENHANG MIT DER SOFTWARE ODER SONSTIGER 
VERWENDUNG DER SOFTWARE ENTSTANDEN.*/
using Microsoft.Xna.Framework;
using PluginSystem.VisualRenderer;
using RapSharp.logic;
using System.Windows;
using System.Windows.Controls;

namespace RapSharp
{
    /// <summary>
    /// Interaktionslogik für cnVisual.xaml
    /// </summary>
    public partial class cnVisual : UserControl
    {
        private bool m_runVisual = true;

        private VisualRenderMenuItem m_curRenderItem = null;

        public float[] ChannelOneData
        {
            get;
            internal set;
        }
        public float[] ChannelTwoData
        {
            get;
            internal set;
        }
        public OutputMenuItem MenuItem
        {
            get;
            internal set;
        }
        public cnVisual()
        {
            InitializeComponent();
        }
        public void RegisterVisualPlugin(IVisualRenderer plugin)
        {
            VisualRenderMenuItem menuitem = new VisualRenderMenuItem(plugin);
            menuitem.Click += menuitem_Click;
            gridMain.ContextMenu.Items.Add(menuitem);

            if (m_curRenderItem == null)
            {
                m_curRenderItem = menuitem;
                m_curRenderItem.IsChecked = true;
                
            }
        }

        void menuitem_Click(object sender, RoutedEventArgs e)
        {
            VisualRenderMenuItem clicked = sender as VisualRenderMenuItem;

            if ((sender as MenuItem) == m_curRenderItem)
            {
                clicked.IsChecked = true;
            }
            else
                m_runVisual = false;

            if (m_runVisual == false)
            {
                m_curRenderItem.Plugin.Stop();
                m_curRenderItem.IsChecked = false;
                m_curRenderItem = clicked;

                m_curRenderItem.Plugin.Start(xnaVisual.RenderSize, MenuItem.Plugin.System);
                m_curRenderItem.IsChecked = true;


                m_runVisual = true;
            }
        }

        private void VisualOff_Click(object sender, RoutedEventArgs e)
        {
            MenuItem clicked = ((MenuItem)sender);

            m_curRenderItem.IsChecked = false;
            m_runVisual = false;
        }

        private void xnaVisual_LoadContent(object sender, XNAControl.GraphicsDeviceEventArgs e)
        {
            foreach (var item in gridMain.ContextMenu.Items)
            {
                if (item is VisualRenderMenuItem)
                {
                    (item as VisualRenderMenuItem).Plugin.LoadContent(e.GraphicsDevice);
                }
            }
            if(MenuItem.Plugin.System != null)
                m_curRenderItem.Plugin.Start(xnaVisual.RenderSize, MenuItem.Plugin.System);
        }

        private void xnaVisual_RenderXna(object sender, XNAControl.GraphicsDeviceEventArgs e)
        {
            if (m_runVisual && this.IsEnabled)
            {
                if (m_curRenderItem != null)
                {
                    m_curRenderItem.Plugin.Update(e.GraphicsDevice);
                    m_curRenderItem.Plugin.Draw(e.GraphicsDevice);
                }
            }
            else
                e.GraphicsDevice.Clear(Color.Black);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void xnaVisual_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var item in gridMain.ContextMenu.Items)
            {
                if (item is VisualRenderMenuItem)
                {
                    (item as VisualRenderMenuItem).Plugin.SizeChanged(e.NewSize);
                }
            }
        }

        private void xnaVisual_RightButtonUp(object sender, XNAControl.MouseEventArgs e)
        {
            gridMain.ContextMenu.PlacementTarget = xnaVisual;
            gridMain.ContextMenu.IsOpen = true;
        }
    }
}
