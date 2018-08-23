(function() {
  'use strict';

  angular.module('BlurAdmin.theme.components')
    .directive('baWizard', baWizard);

  /** @ngInject */
  function baWizard() {
    return {
      restrict: 'E',
      transclude: true,
      templateUrl: '/Common/BaWizard',
      controllerAs: '$baWizardController',
      controller: 'baWizardCtrl'
    }
  }
})();
