(function () {
    "use strict";

    angular.module('AstoveApp').controller('ErrorCtrl', ['$scope', 'AstoveError', 
        function ($scope, $error) {
            $scope.error = $error;

            $scope.$watch('error.hasErrors', function (newVal, oldVal) {
                if (newVal === true) {
                    console.log($scope.error.title);
                    console.log($scope.error.buttonText);
                    console.log($scope.error.hasErrors);
                    console.log($scope.error.errorItems);
                }
            }, true);
        } ]);

    angular.module('AstoveApp').controller('ErrorModalCtrl', ['$scope', '$uibModalInstance', 'AstoveCommon', 'astoveParams',
        function ($scope, $uibModalInstance, $global, astoveParams) {
            $scope.global = $global;
            $scope.message = astoveParams.message;
            $scope.errors = astoveParams.errors;
            $scope.exception = astoveParams.exception;

            $scope.ok = function () {
                $uibModalInstance.close();
            };

            $scope.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };

            $scope.title = function (defaultTitle) {
                return (typeof astoveParams.title !== 'undefined') ? astoveParams.title : defaultTitle;
            };

            $scope.errorList = function () {
                var ul = '<ul class="list-unstyled"><%= items %></ul>';
                var li = '<li><%= error %></li><%= divider %>';
                var templateLi = _.template(li);
                var errorsLi = '';
                var templateUl = _.template(ul);
                var i = 1;
                var divider = '';
                _.forEach($scope.errors, function (error) {
                    divider = '';
                    if ($scope.errors.length !== i)
                        divider = '<li><hr class="divider"></hr></li>';

                    errorsLi += templateLi({ error: error, divider: divider });
                    i += 1;
                });

                return templateUl({ items: errorsLi });
            };
        }]);

})();