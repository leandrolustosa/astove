/**
 * iboxTools - Directive for iBox tools elements in right corner of ibox
 */
function iboxTools($timeout) {
    return {
        restrict: 'A',
        scope: true,
        templateUrl: '/Common/IboxTools',
        controller: ['$scope', '$element', function ($scope, $element) {
            // Function for collapse ibox
            $scope.showhide = function () {
                var ibox = $element.closest('div.ibox');
                var icon = $element.find('i:first');
                var content = ibox.find('div.ibox-content');
                content.slideToggle(200);
                // Toggle icon from up to down
                icon.toggleClass('fa-chevron-up').toggleClass('fa-chevron-down');
                ibox.toggleClass('').toggleClass('border-bottom');
                $timeout(function () {
                    ibox.resize();
                    ibox.find('[id^=map-]').resize();
                }, 50);
            },
            // Function for close ibox
                $scope.closebox = function () {
                    var ibox = $element.closest('div.ibox');
                    ibox.remove();
                }
        }]
    };
 };

function ngAutoComplete ($timeout, $location, $service) {
    return {
        restrict: 'A',
        link: function (scope, elem, attr, ctrl) {
            // elem is a jquery lite object if jquery is not present,
            // but with jquery and jquery ui, it will be a full jquery object.
            $timeout(function () {
                elem.autocomplete({
                    source: function (request, response) {
                        $service.search(request.term).then(function (autocompleteResults) {
                            response($.map(autocompleteResults, function (autocompleteResult) {
                                return {
                                    label: autocompleteResult.name,
                                    value: autocompleteResult
                                }
                            }))
                        });
                    },
                    minLength: 3,
                    select: function (event, selectedItem) {
                        $location.path(selectedItem.item.value.route);
                        scope.$apply();
                        event.preventDefault();
                    }
                });
            });
        }
    };
};
    
    /**
     * icheck - Directive for custom checkbox icheck
     */
    function icheck($timeout) {
        return {
            restrict: 'A',
            require: 'ngModel',
            link: function ($scope, element, $attrs, ngModel) {
                return $timeout(function () {
                    var value;
                    value = $attrs['value'];

                    $scope.$watch($attrs['ngModel'], function (newValue) {
                        $(element).iCheck('update');
                    })

                    return $(element).iCheck({
                        checkboxClass: 'icheckbox_square-green',
                        radioClass: 'iradio_square-green'

                    }).on('ifChanged', function (event) {
                        if ($(element).attr('type') === 'checkbox' && $attrs['ngModel']) {
                            $scope.$apply(function () {
                                return ngModel.$setViewValue(event.target.checked);
                            });
                        }
                        if ($(element).attr('type') === 'radio' && $attrs['ngModel']) {
                            return $scope.$apply(function () {
                                return ngModel.$setViewValue(value);
                            });
                        }
                    });
                });
            }
        };
    }

    function mpValueCopy($parse, $timeout) {
        return function (scope, element, attrs) {
            if (attrs.ngModel) {

                if (element[0].type === "radio") {
                    if (element[0].checked === true) {
                        $parse(attrs.ngModel).assign(scope, element.val());
                    }
                } else {
                    $timeout(function () {
                        $parse(attrs.ngModel).assign(scope, element.val());
                    });
                }
            }
        };
    }

    function imgCropped() {
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
                        nv = nv + "?" + (new Date()).getTime();
                        element.after('<img />');
                        myImg = element.next();
                        myImg.attr('src', nv);
                        console.log(myImg.attr('src'));
                        console.log(nv);
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
    }

    function back($window) {
        return {
            restrict: 'A',
            link: function (scope, elem, attrs) {
                elem.bind('click', function () {
                    $window.history.back();
                });
            }
        };
    }

    function ngMatch($parse) {

        return {
            restrict: 'A',
            require: '?ngModel',
            link: function (scope, elem, attrs, ctrl) {
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
        };
    }

    /**
     *
     * Pass all functions into module
     */
    angular
        .module('AstoveApp')
        .directive('iboxTools', ['$timeout', iboxTools])
        .directive('ngAutoComplete', ['$timeout', '$location', 'AstoveService', ngAutoComplete])
        .directive('icheck', ['$timeout', icheck])
        .directive('mpValueCopy', ['$parse', '$timeout', mpValueCopy])
        .directive('imgCropped', imgCropped)
        .directive('back', ['$window', back])
        .directive('ngMatch', ['$parse', ngMatch]);