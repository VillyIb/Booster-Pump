using System;
using System.Collections.Generic;
using System.Text;

namespace BoosterPumpReducer
{
    public interface IOutputFile
    {
        void OpenFile(string filename);

        public void WriteLine(DateTime timestampLocal, params double[] values);

        void CloseFile();
    }
}
