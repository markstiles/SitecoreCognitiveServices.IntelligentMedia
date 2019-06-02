jQuery.noConflict();

//identify all groups
jQuery(document).ready(function () {

    var identifyAllGroupsForm = ".identify-all-groups-form";
    
    var handleNameValue = jQuery(identifyAllGroupsForm + " #handleName").attr("value");

    jQuery(".form").hide();
    jQuery(".progress-indicator").show();
    
    var timer = setInterval(function() {
        jQuery.post(jQuery(identifyAllGroupsForm).attr("status"), { handleName: handleNameValue })
        .done(function(jobResult) {
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