(function () {
    "use strict";

    angular.module('AstoveApp').controller('MessageBoxCtrl', ['$scope', 'messageBoxService', 
        function ($scope, messageBoxService) {
            $scope.closeMessageBox = function () {
                callMessageBoxFullClose();
                messageBoxService.closed = !messageBoxService.closed;
            };
        } ]);

})();