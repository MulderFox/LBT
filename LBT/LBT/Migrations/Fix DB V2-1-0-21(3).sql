/* PRODUKÈNÍ PROSTØEDÍ */
/* UserId, Poèet mìsícù zdarma, default mìna, CZK, EUR, USD */

/* TODO: Nechat si potvrdit poèáteèní hodnoty */
/* Leoš Èervený */
EXEC SetPaymentsForAccess 1, 3, 0, 790, 28, 35, 1

/* Bedøich Nikl */
EXEC SetPaymentsForAccess 6, 3, 0, 390, 16, 18, 1

/* Zdenìk Mikeska */
EXEC SetPaymentsForAccess 3, 3, 0, 790, 28, 35, 1

/* Ovìøit, že nikdo nemá nulové hodnoty pro platby za pøístup do aplikace */
select * from UserProfile where [ClaAccessYearlyAccessCZK] = 0 or [ClaAccessYearlyAccessEUR] = 0 or [ClaAccessYearlyAccessUSD] = 0