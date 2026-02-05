
(function(document, window, $) {
  'use strict';

  var Site = window.Site;

  $(document).ready(function($) {
    Site.run();
  });

  // Example Wizard Form
  // -------------------

 





  // Example Wizard Progressbar
  // --------------------------
  (function() {
    var defaults = $.components.getDefaults("wizard");

    var options = $.extend(true, {}, defaults, {
      step: '.wizard-pane',
      onInit: function() {
        this.$progressbar = this.$element.find('.progress-bar').addClass('progress-bar-striped');
      },
      onBeforeShow: function(step) {
        step.$element.tab('show');
      },
      onFinish: function() {
        this.$progressbar.removeClass('progress-bar-striped').addClass('progress-bar-success');
      },
      onAfterChange: function(prev, step) {
        var total = this.length();
        var current = step.index + 1;
        var percent = (current / total) * 100;

        this.$progressbar.css({
          width: percent + '%'
        }).find('.sr-only').text(current + '/' + total);
      },
      buttonsAppendTo: '.panel-body'
    });

    $("#exampleWizardProgressbar").wizard(options);
  })();

  // Example Wizard Tabs
  // -------------------
  (function() {
    var defaults = $.components.getDefaults("wizard");
    var options = $.extend(true, {}, defaults, {
      step: '> .nav > li > a',
      onBeforeShow: function(step) {
        step.$element.tab('show');
      },
      classes: {
        step: {
          //done: 'color-done',
          error: 'color-error'
        }
      },
      onFinish: function() {
        alert('finish');
      },
      buttonsAppendTo: '.tab-content'
    });

    $("#exampleWizardTabs").wizard(options);
  })();

  // Example Wizard Accordion
  // ------------------------
  (function() {
    var defaults = $.components.getDefaults("wizard");
    var options = $.extend(true, {}, defaults, {
      step: '.panel-title[data-toggle="collapse"]',
      classes: {
        step: {
          //done: 'color-done',
          error: 'color-error'
        }
      },
      templates: {
        buttons: function() {
          return '<div class="panel-footer">' + defaults.templates.buttons.call(this) + '</div>';
        }
      },
      onBeforeShow: function(step) {
        step.$pane.collapse('show');
      },

      onBeforeHide: function(step) {
        step.$pane.collapse('hide');
      },

      onFinish: function() {
        alert('finish');
      },

      buttonsAppendTo: '.panel-collapse'
    });

    $("#exampleWizardAccordion").wizard(options);
  })();

})(document, window, jQuery);
