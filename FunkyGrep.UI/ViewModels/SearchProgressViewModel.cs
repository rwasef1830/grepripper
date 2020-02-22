#region License
// Copyright (c) 2020 Raif Atef Wasef
// This source file is licensed under the  MIT license.
// 
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to permit persons to whom
// the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY
// KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS
// OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT
// OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion

using Prism.Mvvm;

namespace FunkyGrep.UI.ViewModels
{
    public class SearchProgressViewModel : BindableBase
    {
        long _searchedFileCount;
        long _failedFileCount;
        long _skippedFileCount;
        long? _totalFileCount;

        public long SearchedFileCount
        {
            get => this._searchedFileCount;
            set => this.SetProperty(ref this._searchedFileCount, value);
        }

        public long FailedFileCount
        {
            get => this._failedFileCount;
            set => this.SetProperty(ref this._failedFileCount, value);
        }

        public long SkippedFileCount
        {
            get => this._skippedFileCount;
            set => this.SetProperty(ref this._skippedFileCount, value);
        }

        public long? TotalFileCount
        {
            get => this._totalFileCount;
            set
            {
                if (this.SetProperty(ref this._totalFileCount, value))
                {
                    this.RaisePropertyChanged(nameof(this.TotalFileCountIsSet));
                }
            }
        }

        public bool TotalFileCountIsSet => this.TotalFileCount.HasValue;
    }
}
