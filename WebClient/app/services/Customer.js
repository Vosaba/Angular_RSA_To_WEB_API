angular.module('MyApp')
       .factory('Customer', ['$q', '$http', function ($q, $http) {

           var baseUrl = 'http://localhost:47282/api/customer/';
           var contactBaseUrl = 'http://localhost:47282/api/Contact/';

           var customerService = {};
           customerService.customers = [];
           customerService.currentCustomer = {};

           // Search Customers
           customerService.search = function (text,n,e) {
               var deferred = $q.defer();
               return $http({
                   url: baseUrl + 'search',
                   method: 'GET',
                   params: { 'searchText': text,'nkey': n,'ekey':e },
                   cache: true
               }).success(function (data) {
                   deferred.resolve(
                       customerService.customers = data);
               }).error(function (error) {
                   deferred.reject(error);
               })
               return deferred.promise;
           }

           // New Customers
           customerService.newCustomer = function () {
               var deferred = $q.defer();
               return $http.get(baseUrl + "new")
                    .success(function (data) {
                        deferred.resolve(customerService.customer = data);
                    })
               .error(function (error) {
                   deferred.reject(error);
               })
               return deferred.promise;
           }


           customerService.getKey = function () {
               var deferred = $q.defer();
               return $http.get(baseUrl + "getKey")
                    .success(function (data) {
                        deferred.resolve(customerService.Keys = data);
                    })
               .error(function (error) {
                   deferred.reject(error);
               })
               return deferred.promise;
           }

           // Save Customer
           customerService.Save = function (customer, files) {
               var deferred = $q.defer();
               return $http.post(baseUrl + "Save", customer)
                .success(function (data) {
                    deferred.resolve(customerService.customer = data);
                })

               .error(function (error) {
                   deferred.reject(error);
               });
               return deferred.promise;
           }

           // Customers detail by customer id
           customerService.customerDetail = function (id) {
               var deferred = $q.defer();
               return $http.get(baseUrl + "detail/" + id)
                    .success(function (data) {
                        deferred.resolve(
                            customerService.currentCustomer = data);
                    })
               .error(function (error) {
                   deferred.reject(error);
               })
               return deferred.promise;
           }

           // delete Customers
           customerService.delete = function (id) {
               var deferred = $q.defer();
               return $http.post(baseUrl + "delete/" + id)
                    .success(function (data) {
                        deferred.resolve();
                    })
               .error(function (error) {
                   deferred.reject(error);
               })
               return deferred.promise;
           }
                      
           /*       CUSTOMER CONTACTS
            ************************************/
           customerService.customerContacts = function (id) {
               var deferred = $q.defer();
               return $http.get(contactBaseUrl + "ByCustomerId/" + id)
                    .success(function (data) {
                        deferred.resolve(customerService.conntacts = data);
                    }).error(function (error) {
                        deferred.reject(error);
                    })
               return deferred.promise;
           }

           return customerService;
       }]);