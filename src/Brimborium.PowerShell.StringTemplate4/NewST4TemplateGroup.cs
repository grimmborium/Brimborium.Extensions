using System.Management.Automation;

using Antlr4.StringTemplate;

namespace Brimborium.PowerShell.StringTemplate4 {
    [Cmdlet(VerbsCommon.New, "ST4TemplateGroup")]
    [OutputType(typeof(ST4TemplateGroup))]
    public class NewST4TemplateGroup : PSCmdlet {
        [Parameter(
            Mandatory = false,
            Position = 0,
            ValueFromPipeline = true)]
        public ST4TemplateGroup[] Input { get; set; }

        [Parameter(
            Mandatory = false,
            Position = 1)]
        public ST4TemplateGroup TemplateGroup { get; set; }

        // This method gets called once for each cmdlet in the pipeline when the pipeline starts executing
        protected override void BeginProcessing() {
            //WriteVerbose("Begin!");
        }

        // This method will be called for each input received from the pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord() {
            var result = this.TemplateGroup;
            if (result is null) {
                result = new ST4TemplateGroup();
            }
            if (result.TemplateGroup is null) {
                result.TemplateGroup = new TemplateGroup();
            }

            if (this.Input is object && this.Input.Length > 0) {
                foreach (var element in this.Input) {
                    if (result.TemplateGroup is null) {
                        result.TemplateGroup = element.TemplateGroup;
                    } else {
                        result.TemplateGroup.ImportTemplates(element.TemplateGroup);
                    }
                }
            }

            //var render = new Antlr4.StringTemplate.StringRenderer();
            //render.
            WriteObject(result);
        }

        // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing() {
            //WriteVerbose("End!");
        }
    }
}
