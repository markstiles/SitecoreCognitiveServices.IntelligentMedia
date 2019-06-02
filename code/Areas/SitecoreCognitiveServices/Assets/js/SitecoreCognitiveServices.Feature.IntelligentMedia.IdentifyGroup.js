jQuery.noConflict();

//identify group
jQuery(document).ready(function () {

    var identifyGroupForm = ".identify-group-form";

    var handleNameValue = jQuery(identifyGroupForm + " #handleName").attr("value");

    jQuery(".form").hide();
    jQuery(".progress-indicator").show();

    var timer = setInterval(function() {
        jQuery.post(jQuery(identifyGroupForm).attr("status"), { handleName: handleNameValue })
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