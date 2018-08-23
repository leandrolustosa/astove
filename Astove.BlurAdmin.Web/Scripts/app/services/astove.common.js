(function () {
    "use strict";

    angular.module('AstoveApp').service('AstoveCommon', [
        function () {
            this.loading = true;
            this.loadingData = true;
            this.messageType = '';
            this.message = '';
            this.showAlert = false;
            this.enableQueryString = true;
            this.collapseMenu = (window.localStorage.getItem('collapseMenu') === null) ? false : window.localStorage.getItem('collapseMenu') === 'true';
            this.collapseMenuClass = (window.localStorage.getItem('collapseMenuClass') === null) ? '' : window.localStorage.getItem('collapseMenuClass');
            this.skinClass = (window.localStorage.getItem('skinClass') === null) ? '' : window.localStorage.getItem('skinClass');            
        }]);
})();