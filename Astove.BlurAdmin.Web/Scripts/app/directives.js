(function () {
    "use strict";

    var directives = angular.module('directives', []);

    directives.directive('mpValueCopy', ['$parse', function ($parse) {
        return function (scope, element, attrs) {
            if (attrs.ngModel) {

                if (element[0].type === "radio") {
                    if (element[0].checked === true) {
                        $parse(attrs.ngModel).assign(scope, element.val());
                    }
                } else {
                    $parse(attrs.ngModel).assign(scope, element.val());
                }
            }
        };
    }]);

    directives.directive('bsTooltip', function(){
    return {
        restrict: 'A',
        link: function(scope, element, attrs){
            $(element).hover(function(){
                // on mouseenter
                $(element).tooltip('show');
            }, function(){
                // on mouseleave
                $(element).tooltip('hide');
            });
        }
    };
});

    //directives.directive('uiMask', [
    //          function () {
    //              return {
    //                  require: 'ngModel',
    //                  link: function ($scope, element, attrs, controller) {

    //                      /* We override the render method to run the jQuery mask plugin
    //                      */
    //                      controller.$render = function () {
    //                          var value = controller.$viewValue || '';
    //                          element.val(value);
    //                          element.maskInput($scope.$eval(attrs.uiMask));
    //                      };

    //                      /* Add a parser that extracts the masked value into the model but only if the mask is valid
    //                      */
    //                      controller.$parsers.push(function (value) {
    //                          //var isValid = element.data('isUnmaskedValueValid');
    //                          var isValid = value.indexOf('_') === -1;
    //                          controller.$setValidity('mask', isValid);
    //                          return isValid ? value : undefined;
    //                      });

    //                      /* When keyup, update the view value
    //                      */
    //                      element.bind('keyup', function () {
    //                          $scope.$apply(function () {
    //                              controller.$setViewValue(element.val());
    //                          });
    //                      });
    //                  }
    //              };
    //          }
    //]);

    directives.directive('imgCropped', function () {
        return {
            restrict: 'E',
            replace: true,
            scope: { src: '@', selected: '&', aspectRatio: '@', minSize: '@', maxSize: '@' },
            link: function (scope, element, attr) {
                var myImg;
                var clear = function () {
                    if (myImg) {
                        myImg.next().remove();
                        myImg.remove();
                        myImg = undefined;
                    }
                };
                scope.$watch('src', function (nv) {
                    clear();
                    if (nv) {
                        element.after('<img />');
                        myImg = element.next();
                        myImg.attr('src', nv);
                        console.log(typeof myImg);
                        console.log(typeof jQuery(myImg));
                        console.log(typeof jQuery(myImg).Jcrop);
                        jQuery(myImg).Jcrop({
                            trackDocument: true,
                            onSelect: function (x) {
                                scope.$apply(function () {
                                    scope.selected({ cords: x });
                                });
                            },
                            aspectRatio: attr['aspectRatio'],
                            minSize: JSON.parse(attr['minSize']),
                            maxSize: JSON.parse(attr['maxSize'])
                        });
                    }
                });

                scope.$on('$destroy', clear);
            }
        };
    });

    directives.directive('back', ['$window', function ($window) {
        return {
            restrict: 'A',
            link: function (scope, elem, attrs) {
                elem.bind('click', function () {
                    $window.history.back();
                });
            }
        };
    }]);

    var directiveId = 'ngMatch';
    directives.directive(directiveId, ['$parse', function ($parse) {

        var directive = {
            link: link,
            restrict: 'A',
            require: '?ngModel'
        };
        return directive;

        function link(scope, elem, attrs, ctrl) {
            // if ngModel is not defined, we don't need to do anything
            if (!ctrl) return;
            if (!attrs[directiveId]) return;

            var firstPassword = $parse(attrs[directiveId]);

            var validator = function (value) {
                var temp = firstPassword(scope), v = value === temp;
                ctrl.$setValidity('match', v);
                return value;
            }

            ctrl.$parsers.unshift(validator);
            ctrl.$formatters.push(validator);
            attrs.$observe(directiveId, function () {
                validator(ctrl.$viewValue);
            });

        }
    }]);
})();