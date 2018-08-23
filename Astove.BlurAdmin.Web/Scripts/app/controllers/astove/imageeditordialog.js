(function () {
    "use strict";

    angular.module('AstoveApp').controller('ImageEditorDialogCtrl', ['$scope', 'AstoveFile', 'AstoveData',
        function ($scope, $file, $data) {
            $scope.service = $file;
            $scope.data = $data;
        } ]);

})();