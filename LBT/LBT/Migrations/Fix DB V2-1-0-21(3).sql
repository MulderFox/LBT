/* PRODUK�N� PROST�ED� */
/* UserId, Po�et m�s�c� zdarma, default m�na, CZK, EUR, USD */

/* TODO: Nechat si potvrdit po��te�n� hodnoty */
/* Leo� �erven� */
EXEC SetPaymentsForAccess 1, 3, 0, 790, 28, 35, 1

/* Bed�ich Nikl */
EXEC SetPaymentsForAccess 6, 3, 0, 390, 16, 18, 1

/* Zden�k Mikeska */
EXEC SetPaymentsForAccess 3, 3, 0, 790, 28, 35, 1

/* Ov��it, �e nikdo nem� nulov� hodnoty pro platby za p��stup do aplikace */
select * from UserProfile where [ClaAccessYearlyAccessCZK] = 0 or [ClaAccessYearlyAccessEUR] = 0 or [ClaAccessYearlyAccessUSD] = 0