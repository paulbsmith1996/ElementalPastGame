using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.Rendering
{
    public interface IPictureBoxManager
    {
        public void UpdateBitmapForIRenderingModel(RenderingModel renderingModel);
        public Bitmap ActiveScene { get; }
        public static IPictureBoxManager GetInstance() => throw new NotImplementedException();
        public void AddIPictureBoxManagerObserver(IPictureBoxManagerObserver observer);
        public void RemoveIPictureBoxManagerObserver(IPictureBoxManagerObserver observer);
        public void Redraw();
    }

    public interface IPictureBoxManagerObserver
    {
        public void IPictureBoxManagerNeedsRedraw();
    }
}