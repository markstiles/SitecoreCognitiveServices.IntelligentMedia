jQuery.noConflict();

//analyze all
jQuery(document).ready(function () {
    //handles analyze all form
    var analyzeAllForm = ".analyze-all-form";
    jQuery(analyzeAllForm + " button")
        .click(function (event) {
            event.preventDefault();

            var idValue = jQuery(analyzeAllForm + " #id").attr("value");
            var langValue = jQuery(analyzeAllForm + " #language").attr("value");
            var dbValue = jQuery(analyzeAllForm + " #db").attr("value");
            var overwriteValue = jQuery(analyzeAllForm + " #overwrite").is(":checked");

            jQuery(".form").hide();
            jQuery(".progress-indicator").show();
            
            jQuery.post(
                jQuery(analyzeAllForm).attr("action"),
                {
                    id: idValue,
                    language: langValue,
                    db: dbValue,
                    overwrite: overwriteValue
                }
            ).done(function (r) {
                var timer = setInterval(function () {
                    jQuery.post(jQuery(analyzeAllForm).attr("status"), { handleName: r })
                        .done(function (jobResult) {
                            if (jobResult.Total < 0) {
                                jQuery(".result-waiting").show();
                                
                                return;
                            }

                            jQuery(".result-count").text(jobResult.Current + " of " + jobResult.Total);
                            jQuery(".result-display").show();
                            jQuery(".result-waiting").hide();

                            if (jobResult.Completed) {
                                jQuery(".progress-indicator").hide();
                                clearInterval(timer);    
                            }
                        });
                }, 1000);
            });
        });
});