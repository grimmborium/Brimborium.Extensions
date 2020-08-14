using System.Management.Automation;

using Antlr4.StringTemplate;

namespace Brimborium.PowerShell.StringTemplate4 {
    [Cmdlet(VerbsCommon.New, "ST4TemplateGroupFile")]
    [OutputType(typeof(ST4TemplateGroup))]
    public class NewST4TemplateGroupFile : PSCmdlet {
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true)]
        public string FilePath { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 1)]
        public ST4TemplateGroup TemplateGroup { get; set; }

        //[Parameter(
        //    Position = 1,
        //    ValueFromPipelineByPropertyName = true)]
        //[ValidateSet("Cat", "Dog", "Horse")]
        //public string FavoritePet { get; set; } = "Dog";

        // This method gets called once for each cmdlet in the pipeline when the pipeline starts executing
        protected override void BeginProcessing() {
            //WriteVerbose("Begin!");
        }

        // This method will be called for each input received from the pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord() {
            var templateGroup = new TemplateGroupFile(this.FilePath);
            ST4TemplateGroup result = this.TemplateGroup;
            if (result is null) {
                result = new ST4TemplateGroup();
                result.TemplateGroup = templateGroup;
            } else {
                result.TemplateGroup.ImportTemplates(templateGroup);
            }
            WriteObject(result);
        }

        // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing() {
            //WriteVerbose("End!");
        }
    }
}
