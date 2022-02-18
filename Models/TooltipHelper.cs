using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace pz3.Models
{
    public class ToolTipHelper
    {
        private ToolTip _toolTip;

        public ToolTipHelper(string tooltipMessage)
        {
            _toolTip = new ToolTip();
            _toolTip.Content = tooltipMessage;
        }
        public ToolTip ToolTip { get; set; }

        public void turnOn(string text)
        {
            if (_toolTip == null)
            {
                _toolTip = new ToolTip();
            }
            _toolTip.Content = text;
            _toolTip.Dispatcher.Invoke(new Action(() => { _toolTip.IsOpen = true; }));
        }

        public void turnOff()
        {
            if (_toolTip != null)
                _toolTip.IsOpen = false;
        }

    }
}