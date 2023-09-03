(function ($) {
    $.sstm_decrypt = function (encryptedTxt) {
        let result = '';
        let key = 328477227719771;
        encryptedTxt = atob(encryptedTxt);
        for (let i = 0; i < encryptedTxt.length; i++) {
            let charCode = encryptedTxt[i].charCodeAt();
            if (charCode > 96 && charCode < 123) {
                charCode += key % 26
                if (charCode > 122) {
                    charCode = (charCode - 122) + 96;
                } else if (charCode < 97) {
                    charCode = (charCode - 97) + 123;
                }
            }
            if (charCode > 64 && charCode < 91) {
                charCode += key % 26
                if (charCode > 90) {
                    charCode = (charCode - 90) + 64;
                } else if (charCode < 65) {
                    charCode = (charCode - 65) + 91;
                }
            }
            result += String.fromCharCode(charCode);
        }
        return result;
    }
})(jQuery);

(function ($) {
    $.sstm_encrypt = function (plainTxt) {
        let result = '';
        let key = 328477227719771;
        for (let i = 0; i < plainTxt.length; i++) {
            let charCode = plainTxt[i].charCodeAt();
            if (charCode > 96 && charCode < 123) {
                charCode += key % 26
                if (charCode > 122) {
                    charCode = (charCode - 122) + 96;
                } else if (charCode < 97) {
                    charCode = (charCode - 97) + 123;
                }
            }
            if (charCode > 64 && charCode < 91) {
                charCode += key % 26
                if (charCode > 90) {
                    charCode = (charCode - 90) + 64;
                } else if (charCode < 65) {
                    charCode = (charCode - 65) + 91;
                }
            }
            result += String.fromCharCode(charCode);
        }
        return btoa(result)
    }
})(jQuery);