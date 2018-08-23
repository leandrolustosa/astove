(function () {
    "use strict";

    function range() {
        return function (input, start, total) {
            if (typeof start === 'undefined')
                start = 0;
                
            total = parseInt(total);
            for (var i = start; i < total; i++)
                input.push(i);
            return input;
        };
    }

    angular.module('AstoveApp')
        .filter('range', range)

})();