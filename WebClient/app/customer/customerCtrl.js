var app = angular.module("MyApp")
app.controller('customerCtrl', ['$scope', '$state', '$stateParams', '$modal', '$log', 'Customer', function ($scope, $state, $stateParams, $modal, $log, Customer) {

    var customerId = $stateParams.customerId;

    $scope.searchText = '';
    $scope.customers = searchCustomers();
    $scope.contacts = [];
    $scope.customer = null;
    $scope.currentCustomer = null;


    $scope.$watch('searchText', function (newVal, oldVal) {
        if (newVal != oldVal) {
            searchCustomers();
        }
    }, true);


    function searchCustomers() {

        var rsa = new Rsa();
      var keys=  rsa.GetPublicKey();
        Customer.search($scope.searchText, keys.N,keys.E)
        .then(function (data) {

            ////for (var ind in Customer.customers) {

            ////    var rsaCustomer = Customer.customers[ind];

            ////    for (var item in rsaCustomer) {
            ////        if (rsaCustomer[item] && rsaCustomer[item] != null)
            ////            rsaCustomer[item] = rsa.Decode(rsaCustomer[item], keys.N, keys.D);
            ////    }


            ////}

            $scope.customers = Customer.customers;
        });
    };

    $scope.customerDetail = function (id) {
        if (!id) return;
        Customer.customerDetail(id)
        .then(function (data) {
            $scope.currentCustomer = Customer.currentCustomer;
            $state.go('customer.detail', { 'customerId': id });
        });
    };

    /* Need to call after defining the function
       It will be called on page refresh        */
    $scope.currentCustomer = $scope.customerDetail(customerId);

    // Delete a customer and hide the row
    $scope.deleteCustomer = function ($event, id) {
        var ans = confirm('Are you sure to delete it?');
        if (ans) {
            Customer.delete(id)
            .then(function () {
                var element = $event.currentTarget;
                $(element).closest('div[class^="col-lg-12"]').hide();
            })
        }
    };

    // Add Customer
    $scope.addCustomer = function () {

       

        Customer.newCustomer()
        .then(function (data) {
            $scope.customer = Customer.customer;
            $scope.open('sm');
        });
    }

    // Edit Customer
    $scope.editCustomer = function () {
        $scope.customer = $scope.currentCustomer;
        $scope.open('lg');
    }

    // Open model to add edit customer
    $scope.open = function (size) {
        var modalInstance = $modal.open({
            animation: false,
            backdrop: 'static',
            templateUrl: 'app/customer/AddEditCustomer.html',
            controller: 'customerModalCtrl',
            size: size,
            resolve: {
                customer: function() {
                    return $scope.customer;
                }
            }
        });

        modalInstance.result.then(function (response) {
            $scope.currentCustomer = response;
            $scope.customers.push(response);
            $state.go('customer.detail', { 'customerId': response.CustomerId });            
        }, function () {
            $log.info('Modal dismissed at: ' + new Date());
        });
    };


    
}]);

app.controller('customerModalCtrl', ['$scope', '$modalInstance', 'Customer', 'customer', function ($scope, $modalInstance, Customer, customer) {

    $scope.customer = customer;
    
    if (customer.CustomerId > 0)
        $scope.headerTitle = 'Edit Customer';
    else
        $scope.headerTitle = 'Add Customer';
    
    $scope.save = function () {
        Customer.getKey().then(function (Keys) {
            debugger;

            var rsaCustomer = angular.copy($scope.customer);
            var rsa = new Rsa();

            for (var item in rsaCustomer) {
                if (rsaCustomer[item] && rsaCustomer[item]!=null)
                rsaCustomer[item] = rsa.Encode(rsaCustomer[item], Keys.data.Ekey, Keys.data.Nkey);
            }
            rsaCustomer.CustomerId = $scope.customer.CustomerId;
            Customer.Save(rsaCustomer).then(function (response) {
                $modalInstance.close(response.data);
                //location.reload();

            })

        });
       
    };

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };

}]);


var Rsa = function () {

    var self = this;


    var p; //destroy
    var q; //destroy
    var phi; //destroy
    var n;
    var e;
    var d;

    function stringToBytes(str) {
        var ch, st, re = [];
        for (var i = 0; i < str.length; i++) {
            ch = str.charCodeAt(i);  // get char 
            st = [];                 // set up "stack"
            do {
                st.push(ch & 0xFF);  // push byte to stack
                ch = ch >> 8;          // shift value down by 1 byte
            }
            while (ch);
            // add stack contents to result
            // done because chars have "wrong" endianness
            re = re.concat(st.reverse());
        }
        // return an array of bytes
        return re;
    }

    function bin2String(array) {
        return String.fromCharCode.apply(String, array);
    }

    self.Encode = function (text, initE, initN) {
        if (!initE) {
            InitKeyData();
        } else {
            e = initE;
            n = initN;
        }
        var outStr = "";

        //System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
        //byte[] strBytes = Encoding.UTF8.GetBytes(text);


        var strBytes = stringToBytes(text);

        for (value in strBytes) {
            var encryptedValue = ModuloPow(strBytes[value], e, n);
            //var encryptedValue = ModuloPow(strBytes[value],3709, 6731);
            outStr += encryptedValue + "|";
            d = d;
        }

        return outStr;
    }

    self.Decode = function (text,  ns,  ds)
    {
        //text = Encoding.UTF8.GetString(Convert.FromBase64String(text));

        //if (ns) {
        //    n = ns;
        //    d = ds;
        //}
        var outStr = "";
        var arr = GetDecArrayFromText(text);
        var bytes = [];
        // System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
        var j = 0;
        for (var i in arr) {
            var decryptedValue = ModuloPow(arr[i], d, n);

            bytes[j] = decryptedValue;
            j++;

        }
        outStr += bin2String(bytes);
        return outStr;
    }


    function GetDecArrayFromText(text) {

        var result = [];
        var i = 0;

        var tmp = "";

        for (var c in text) {
            if (text[c] != '|') {
                tmp += text[c];
            }
            else {
                result[i] = parseInt(tmp);
                i++;
                tmp = "";
            }
        }

        return result;
    }

    self.GetPublicKey = function() {
        InitKeyData();

        return {
            N: n,
            E: e,
            D: d
        };
    }

    function InitKeyData() {
        //Random random = new Random();

        var simple = GetNotDivideable();
        p = simple[getRandomInt(0, simple.length)];
        q = simple[getRandomInt(0, simple.length)];
        n = p * q;
        phi = (p - 1) * (q - 1);
        var possibleE = GetAllPossibleE(phi);

        do {
            e = possibleE[getRandomInt(0, possibleE.length)];
            d = ExtendedEuclide(e % phi, phi).u1;
        } while (d < 0);
    }


    function GetAllPossibleE(phi) {
        var result = [];

        for (var i = 2; i < phi; i++) {
            if (ExtendedEuclide(i, phi).gcd == 1) {
                result.push(i);
            }
        }

        return result;
    }

    function ResObj(u1, res, u3) {
        this.u1 = u1;
        this.u2 = res;
        this.gcd = u3;
    }

    function ExtendedEuclide(a, b) {
        var u1 = 1;
        var u3 = a;
        var v1 = 0;
        var v3 = b;

        while (v3 > 0) {
            var q0 = Math.floor(u3 / v3);
            var q1 = u3 % v3;

            var tmp = v1 * q0;
            var tn = u1 - tmp;
            u1 = v1;
            v1 = tn;

            u3 = v3;
            v3 = q1;
        }

        var tmp2 = u1 * a;
        tmp2 = u3 - (tmp2);
        var res = Math.floor(tmp2 / b);

        var result = new ResObj(u1, res, u3);


        return result;
    }

    function GetNotDivideable() {
        var notDivideable = [];

        for (var x = 2; x < 256; x++) {
            var n = 0;
            for (var y = 1; y <= x; y++) {
                if (x % y == 0)
                    n++;
            }

            if (n <= 2)
                notDivideable.push(x);
        }
        return notDivideable;
    }

    function getRandomInt(min, max) {
        return Math.floor(Math.random() * (max - min + 1)) + min;
    }


    function ModuloPow(value, pow, modulo) {
        var result = value;
        for (i = 0; i < pow - 1; i++) {
            result = (result * value) % modulo;
        }
        return result;
    }

}
