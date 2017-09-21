var LBT = LBT || {};
LBT.LocalizedResources = LBT.LocalizedResources || {};

LBT.LocalizedResources.LanguageName = "1029";

LBT.LocalizedResources.GetString = function (resourceKey) {
    if (LBT.LocalizedResources.hasOwnProperty(LBT.LocalizedResources.LanguageName)) {
        return LBT.LocalizedResources[LBT.LocalizedResources.LanguageName][resourceKey];
    } else {
        return LBT.LocalizedResources["1029"][resourceKey];
    }
};

LBT.LocalizedResources["1029"] = {
    "UserWillBeLogoutMessage": "V rámci Vaší nečinnosti budete automaticky odhlášen za {0} sekund",
    "UnsignUserQuestion": "Opravdu chcete přihlášeného účastníka odhlásit?"
};