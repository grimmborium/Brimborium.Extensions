using System.Management.Automation;
using System.Linq;
using System.Runtime.CompilerServices;
using System;

namespace Brimborium.PowerShell.StringTemplate4 {
    [Cmdlet(VerbsData.Save, "ST4TemplateOutput")]
    [OutputType(typeof(ST4TemplateOutput))]
    public class SaveST4TemplateOutputCmdlet : PSCmdlet {
        
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true)]
        public ST4TemplateOutput[] Input { get; set; }
        
        //    ValueFromPipelineByPropertyName = true

        [Parameter(
            Position = 1)]
        public string BasePath { get; set; }

        [Parameter(
            Position = 2)]
        public string FullName { get; set; }

        protected override void BeginProcessing() {
            //WriteVerbose("Begin!");
            if (System.Diagnostics.Debugger.IsAttached) {
                System.Diagnostics.Debugger.Break();
            } else { 
                System.Diagnostics.Debugger.Launch();
            }
        }

        // This method will be called for each input received from the pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord() {
            //var pathInfoBasePath = (!string.IsNullOrEmpty(this.BasePath))
            //    ? SessionState.Path.GetResolvedPSPathFromPSPath(this.BasePath)?.FirstOrDefault()
            //    : SessionState.Path.CurrentLocation
            //    ;
            var basePath = (!string.IsNullOrEmpty(this.BasePath))
                ? this.BasePath
                : SessionState.Path.CurrentLocation.Path
                ;
            if(this.Input is null){
                return;
            }

            //this.InvokeCommand.GetCmdlet("Get-Conntent").

            //var scriptBlock = this.InvokeCommand.NewScriptBlock("{param($path) Get-Content -Path $path -Raw }");
            //scriptBlock.Invoke()
            //this.InvokeCommand.InvokeScript("{param($path) Get-Content -Path $path -Raw }", 1);
            if (!string.IsNullOrEmpty(this.FullName)) {
                if (this.Input.Length > 1) {
                    this.WriteError(new ErrorRecord(new ArgumentException("Use FullName with only one Input"), null, ErrorCategory.InvalidArgument, null));
                    return;
                } else {
                    return;
                }
            } else { 
                foreach (var inItem in this.Input) {
                    string fileName = inItem.FileName;
                    //SessionState.Path.Combine(basePath, inItem.FileName);
                    
                    //contentStreams = GetContentWriters(paths, currentContext);
                }
            }
            //this.WriteObject(pathInfoBasePath);

            //PathInfo basePathInfo = 
            //    !string.IsNullOrEmpty(this.BasePath) 
            //    ? SessionState.Path.GetResolvedPSPathFromPSPath(this.BasePath)
            //    : SessionState.Path.CurrentLocation;
            //SessionState.Path.Combine(Cur)
            //SessionState.Path.GetResolvedPSPathFromPSPath()
            //this.InvokeProvider.Content.GetReader

            //WriteObject(new FavoriteStuff {
            //    FavoriteNumber = FavoriteNumber,
            //    FavoritePet = FavoritePet
            //});
        }

        // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing() {
            //WriteVerbose("End!");
        }
    }

}
