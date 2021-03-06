﻿using System.Management.Automation;

using Antlr4.StringTemplate;

namespace Brimborium.PowerShell.StringTemplate4 {
    [Cmdlet(VerbsCommon.New, "ST4TemplateOutput")]
    [OutputType(typeof(ST4TemplateOutput))]
    public class NewST4TemplateOutputCmdlet : PSCmdlet {
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public int FavoriteNumber { get; set; }

        [Parameter(
            Position = 1,
            ValueFromPipelineByPropertyName = true)]
        [ValidateSet("Cat", "Dog", "Horse")]
        public string FavoritePet { get; set; } = "Dog";

        // This method gets called once for each cmdlet in the pipeline when the pipeline starts executing
        protected override void BeginProcessing() {
            WriteVerbose("Begin!");
        }

        // This method will be called for each input received from the pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord() {

            //var render = new Antlr4.StringTemplate.StringRenderer();
            //render.
            WriteObject(new FavoriteStuff {
                FavoriteNumber = FavoriteNumber,
                FavoritePet = FavoritePet
            });
        }

        // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing() {
            WriteVerbose("End!");
        }
    }
}
