using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public static class ParserExportacionDropDown
{
    public static string NombreCodigoModalidadVenta(int _CodModVenta)
    {
        switch (_CodModVenta)
        {
            case 1:
                return "A FIRME (FIRME)";
            case 2:
                return "BAJO CONDICION (BAJO COND.)";
            case 3:
                return "EN CONSIGNACION LIBRE (CONS-LIBRE)";
            case 4:
                return "EN CONSIGNACION CON UN MINIMO A FIRME (COND-M.F.)";
            case 9:
                return "SIN PAGO";
            default:
                return "Codigo Modalidad de Venta";
        }
    }

    public static string NombreIndicadorServicio(int _CodIndServ)
    {
        switch (_CodIndServ)
        {
            case 1:
                return "Factura de servicios periódicos domiciliarios";
            case 2:
                return "Factura de otros servicios periódicos";
            case 3:
                return "Factura de Servicios";
            case 4:
                return "Servicios de Hotelería";
            case 5:
                return "Servicio de Transporte Terrestre Internacional";
            default:
                return "Codigo Indicador Servicio";
        }
    }
    public static string NombreCodigoPais(int _CodPais)
    {
        switch (_CodPais)
        {
            case 308:
                return "AFGANISTAN";
            case 518:
                return "ALBANIA";
            case 563:
                return "ALEMANIA";
            case 503:
                return "ALEMANIA R.D.(N";
            case 502:
                return "ALEMANIA R.F.";
            case 132:
                return "ALTO VOLTA";
            case 525:
                return "ANDORRA";
            case 140:
                return "ANGOLA";
            case 242:
                return "ANGUILA";
            case 240:
                return "ANTIGUA Y BBUDA";
            case 247:
                return "ANTILLAS NEERLANDESA";
            case 302:
                return "ARABIA SAUDITA";
            case 127:
                return "ARGELIA";
            case 224:
                return "ARGENTINA";
            case 540:
                return "ARMENIA";
            case 243:
                return "ARUBA";
            case 406:
                return "AUSTRALIA";
            case 509:
                return "AUSTRIA";
            case 541:
                return "AZERBAIJAN";
            case 207:
                return "BAHAMAS";
            case 313:
                return "BAHREIN";
            case 321:
                return "BANGLADESH";
            case 204:
                return "BARBADOS";
            case 542:
                return "BELARUS";
            case 514:
                return "BELGICA";
            case 236:
                return "BELICE";
            case 150:
                return "BENIN";
            case 244:
                return "BERMUDAS";
            case 318:
                return "BHUTAN";
            case 221:
                return "BOLIVIA";
            case 154:
                return "BOPHUTHATSWANA";
            case 543:
                return "BOSNIA HEZGVINA";
            case 113:
                return "BOTSWANA";
            case 220:
                return "BRASIL";
            case 344:
                return "BRUNEI";
            case 527:
                return "BULGARIA";
            case 161:
                return "BURKINA FASO";
            case 141:
                return "BURUNDI";
            case 129:
                return "CABO VERDE";
            case 315:
                return "CAMBODIA";
            case 149:
                return "CAMERUN";
            case 226:
                return "CANADA";
            case 130:
                return "CHAD";
            case 529:
                return "CHECOESLOVAQUIA";
            case 997:
                return "CHILE";
            case 336:
                return "CHINA";
            case 305:
                return "CHIPRE";
            case 162:
                return "CISKEY";
            case 202:
                return "COLOMBIA";
            case 901:
                return "COMB.Y LUBRIC.";
            case 118:
                return "COMORAS";
            case 144:
                return "CONGO";
            case 334:
                return "COREA DEL NORTE";
            case 333:
                return "COREA DEL SUR";
            case 107:
                return "COSTA DE MARFIL";
            case 211:
                return "COSTA RICA";
            case 547:
                return "CROACIA";
            case 209:
                return "CUBA";
            case 906:
                return "DEPOSITO FRANCO";
            case 507:
                return "DINAMARCA";
            case 155:
                return "DJIBOUTI";
            case 231:
                return "DOMINICA";
            case 218:
                return "ECUADOR";
            case 124:
                return "EGIPTO";
            case 213:
                return "EL SALVADOR";
            case 341:
                return "EMIR.ARAB.UNID.";
            case 163:
                return "ERITREA";
            case 548:
                return "ESLOVENIA";
            case 517:
                return "ESPANA";
            case 549:
                return "ESTONIA";
            case 139:
                return "ETIOPIA";
            case 401:
                return "FIJI";
            case 335:
                return "FILIPINAS";
            case 512:
                return "FINLANDIA";
            case 505:
                return "FRANCIA";
            case 145:
                return "GABON";
            case 102:
                return "GAMBIA";
            case 550:
                return "GEORGIA";
            case 108:
                return "GHANA";
            case 565:
                return "GIBRALTAR";
            case 585:
                return "GILBRALTAR";
            case 232:
                return "GRANADA";
            case 520:
                return "GRECIA";
            case 253:
                return "GROENLANDIA";
            case 425:
                return "GUAM";
            case 215:
                return "GUATEMALA";
            case 566:
                return "GUERNSEY";
            case 104:
                return "GUINEA";
            case 147:
                return "GUINEA ECUATRL";
            case 103:
                return "GUINEA-BISSAU";
            case 217:
                return "GUYANA";
            case 208:
                return "HAITI";
            case 515:
                return "HOLANDA";
            case 214:
                return "HONDURAS";
            case 342:
                return "HONG KONG";
            case 530:
                return "HUNGRIA";
            case 317:
                return "INDIA";
            case 328:
                return "INDONESIA";
            case 307:
                return "IRAK";
            case 309:
                return "IRAN";
            case 506:
                return "IRLANDA";
            case 567:
                return "ISLA DE MAN";
            case 516:
                return "ISLANDIA";
            case 246:
                return "ISLAS CAYMAN";
            case 427:
                return "ISLAS COOK";
            case 327:
                return "ISLAS MALDIVAS";
            case 424:
                return "ISLAS MARIANAS DEL NORTE";
            case 164:
                return "ISLAS MARSHALL";
            case 418:
                return "ISLAS SALOMON";
            case 403:
                return "ISLAS TONGA";
            case 245:
                return "ISLAS VIRG.BRIT";
            case 249:
                return "ISLAS VIRGENES (ESTADOS UNIDOS";
            case 306:
                return "ISRAEL";
            case 504:
                return "ITALIA";
            case 205:
                return "JAMAICA";
            case 331:
                return "JAPON";
            case 568:
                return "JERSEY";
            case 301:
                return "JORDANIA";
            case 551:
                return "KASAJSTAN";
            case 137:
                return "KENIA";
            case 552:
                return "KIRGISTAN";
            case 416:
                return "KIRIBATI";
            case 303:
                return "KUWAIT";
            case 316:
                return "LAOS";
            case 114:
                return "LESOTHO";
            case 553:
                return "LETONIA";
            case 311:
                return "LIBANO";
            case 106:
                return "LIBERIA";
            case 125:
                return "LIBIA";
            case 534:
                return "LIECHTENSTEIN";
            case 554:
                return "LITUANIA";
            case 532:
                return "LUXEMBURGO";
            case 345:
                return "MACAO";
            case 555:
                return "MACEDONIA";
            case 120:
                return "MADAGASCAR";
            case 329:
                return "MALASIA";
            case 115:
                return "MALAWI";
            case 133:
                return "MALI";
            case 523:
                return "MALTA";
            case 128:
                return "MARRUECOS";
            case 250:
                return "MARTINICA";
            case 119:
                return "MAURICIO";
            case 134:
                return "MAURITANIA";
            case 216:
                return "MEXICO";
            case 417:
                return "MICRONESIA";
            case 556:
                return "MOLDOVA";
            case 535:
                return "MONACO";
            case 337:
                return "MONGOLIA";
            case 252:
                return "MONSERRAT";
            case 561:
                return "MONTENEGRO";
            case 121:
                return "MOZAMBIQUE";
            case 326:
                return "MYANMAR (EX BIR";
            case 998:
                return "NAC.REPUTADA";
            case 159:
                return "NAMIBIA";
            case 402:
                return "NAURU";
            case 320:
                return "NEPAL";
            case 212:
                return "NICARAGUA";
            case 131:
                return "NIGER";
            case 111:
                return "NIGERIA";
            case 421:
                return "NIUE";
            case 513:
                return "NORUEGA";
            case 423:
                return "NUEVA CALEDONIA";
            case 405:
                return "NUEVA ZELANDIA";
            case 304:
                return "OMAN";
            case 904:
                return "ORIG.O DEST. NO";
            case 999:
                return "OTROS(PAIS DESC";
            case 324:
                return "PAKISTAN";
            case 420:
                return "PALAU";
            case 210:
                return "PANAMA";
            case 222:
                return "PARAGUAY";
            case 219:
                return "PERU";
            case 903:
                return "PESCA EXTRA";
            case 422:
                return "POLINESIA FRANCESA";
            case 528:
                return "POLONIA";
            case 501:
                return "PORTUGAL";
            case 412:
                return "PPUA.NVA.GUINEA";
            case 251:
                return "PUERTO RICO";
            case 312:
                return "QATAR";
            case 902:
                return "RANCHO DE NAVES";
            case 510:
                return "REINO UNIDO";
            case 148:
                return "REP.CENT.AFRIC.";
            case 143:
                return "REP.DEM. CONGO";
            case 206:
                return "REP.DOMINICANA";
            case 545:
                return "REP.ESLOVACA";
            case 544:
                return "REPUBLICA CHECA";
            case 546:
                return "REPUBLICA DE SERBIA";
            case 346:
                return "REPUBLICA DE YEMEN";
            case 564:
                return "RF YUGOSLAVIA";
            case 519:
                return "RUMANIA";
            case 562:
                return "RUSIA";
            case 142:
                return "RWANDA";
            case 146:
                return "S.TOM.PRINCIPE";
            case 234:
                return "S.VTE.Y GRANAD.";
            case 165:
                return "SAHARAUI";
            case 404:
                return "SAMOA OCC.";
            case 536:
                return "SAN MARINO";
            case 233:
                return "SANTA LUCIA(ISL";
            case 524:
                return "SANTA SEDE";
            case 101:
                return "SENEGAL";
            case 156:
                return "SEYCHELLES";
            case 105:
                return "SIERRA LEONA";
            case 332:
                return "SINGAPUR";
            case 310:
                return "SIRIA";
            case 241:
                return "SNT.KIT & NEVIS";
            case 138:
                return "SOMALIA";
            case 314:
                return "SRI LANKA";
            case 112:
                return "SUDAFRICA";
            case 123:
                return "SUDAN";
            case 160:
                return "SUDAN DEL SUR";
            case 511:
                return "SUECIA";
            case 508:
                return "SUIZA";
            case 235:
                return "SURINAM";
            case 122:
                return "SWAZILANDIA";
            case 409:
                return "T.NORTEAM.EN AU";
            case 557:
                return "TADJIKISTAN";
            case 330:
                return "TAIWAN (FORMOSA";
            case 135:
                return "TANZANIA";
            case 152:
                return "TER.ESPAN.EN AF";
            case 229:
                return "TER.HOLAN.EN AM";
            case 343:
                return "TER.PORTUG.E/AS";
            case 151:
                return "TERR.BRIT.EN AF";
            case 227:
                return "TERR.BRIT.EN AM";
            case 407:
                return "TERR.BRIT.EN AU";
            case 230:
                return "TERR.D/DINAMARC";
            case 153:
                return "TERR.FRAN.EN AF";
            case 228:
                return "TERR.FRAN.EN AM";
            case 408:
                return "TERR.FRAN.EN AU";
            case 319:
                return "THAILANDIA";
            case 426:
                return "TIMOR ORIENTAL";
            case 109:
                return "TOGO";
            case 166:
                return "TRANSKEI";
            case 203:
                return "TRINID.Y TOBAGO";
            case 126:
                return "TUNEZ";
            case 248:
                return "TURCAS Y CAICOS";
            case 558:
                return "TURKMENISTAN";
            case 522:
                return "TURQUIA";
            case 419:
                return "TUVALU";
            case 521:
                return "U.R.S.S.   (NO";
            case 225:
                return "U.S.A.";
            case 559:
                return "UCRANIA";
            case 136:
                return "UGANDA";
            case 223:
                return "URUGUAY";
            case 560:
                return "UZBEKISTAN";
            case 415:
                return "VANUATU";
            case 201:
                return "VENEZUELA";
            case 158:
                return "VIENDA";
            case 325:
                return "VIETNAM";
            case 322:
                return "YEMEN";
            case 323:
                return "YEMEN DEL SUR";
            case 526:
                return "YUGOESLAVIA (NO";
            case 117:
                return "ZAMBIA";
            case 910:
                return "ZF.ARICA-ZF IND";
            case 905:
                return "ZF.IQUIQUE";
            case 907:
                return "ZF.PARENAS";
            case 116:
                return "ZIMBABWE";
            default:
                return "Codigo Pais";
        }
    }

    public static string NombreCodigoPuerto(int _CodPuerto)
    {
        switch (_CodPuerto)
        {
            case 644: return "AALBORG";
            case 641: return "AARHUS";
            case 960: return "ABRA DE NAPA";
            case 218: return "ACAPULCO";
            case 814: return "ADELAIDA";
            case 992: return "AEROP.A.M.BENITEZ";
            case 991: return "AEROP.C.I.DEL CAMPO";
            case 989: return "AEROP.CERRO MORENO";
            case 987: return "AEROP.CHACALLUTA";
            case 988: return "AEROP.DIEGO ARACENA";
            case 990: return "AEROP.EL TEPUAL";
            case 951: return "AGUAS NEGRAS";
            case 220: return "ALTAMIRA";
            case 601: return "AMBERES";
            case 621: return "AMSTERDAM";
            case 931: return "ANCUD";
            case 903: return "ANTOFAGASTA";
            case 974: return "APPELEG";
            case 901: return "ARICA";
            case 995: return "ARICA-LA PAZ";
            case 994: return "ARICA-TACNA";
            case 546: return "AUGUSTA";
            case 266: return "BAHIA BLANCA";
            case 978: return "BAKER";
            case 222: return "BALBOA";
            case 136: return "BALTIMORE";
            case 563: return "BARCELONA";
            case 233: return "BARRANQUILLA";
            case 146: return "BATON ROUGE";
            case 118: return "BAYSIDE";
            case 533: return "BELGRADO";
            case 564: return "BILBAO";
            case 131: return "BOSTON";
            case 591: return "BREMEN";
            case 557: return "BREST";
            case 133: return "BRIDGEPORT";
            case 158: return "BROWSVILLE";
            case 232: return "BUENAVENTURA";
            case 262: return "BUENOS AIRES";
            case 555: return "BURDEOS";
            case 422: return "BUSAN CY (PUSAN)";
            case 942: return "CABO NEGRO";
            case 562: return "CADIZ";
            case 556: return "CALAIS";
            case 205: return "CALBUCO";
            case 471: return "CALCUTA";
            case 918: return "CALDERA";
            case 919: return "CALDERILLA";
            case 950: return "CALETA COLOSO";
            case 252: return "CALLAO";
            case 993: return "CAP HUACHIPATO";
            case 967: return "CARDENAL SAMORE";
            case 980: return "CASAS VIEJAS";
            case 932: return "CASTRO";
            case 911: return "CHACABUCO/PTO.AYSEN";
            case 957: return "CHACALLUTA";
            case 934: return "CHAITEN";
            case 481: return "CHALNA";
            case 917: return "CHANARAL/BARQUITO";
            case 139: return "CHARLESTON";
            case 977: return "CHILE CHICO";
            case 958: return "CHUNGARA";
            case 712: return "CIUDAD DEL CABO";
            case 940: return "CLARENCIA";
            case 214: return "COATZACOALCOS";
            case 959: return "COLCHANE";
            case 223: return "COLON";
            case 147: return "COLUMBRES";
            case 267: return "COMODORO RIVADAVIA";
            case 511: return "CONSTANZA";
            case 923: return "CONSTUTUCION";
            case 642: return "COPENHAGEN";
            case 904: return "COQUIMBO";
            case 265: return "CORDOBA";
            case 926: return "CORONEL";
            case 157: return "CORPUS CRISTI";
            case 930: return "CORRAL";
            case 121: return "COSTA DEL ATLANTICO, OTROS NO";
            case 112: return "COSTA DEL PACIFICO, OTROS NO E";
            case 123: return "COSTA DEL PACIFICO, OTROS NO E";
            case 212: return "COSTA DEL PACIFICO, OTROS PUER";
            case 983: return "COYHAIQUE ALTO";
            case 221: return "CRISTOBAL";
            case 302: return "CURAZAO";
            case 938: return "CUTTER COVE";
            case 597: return "CUXHAVEN";
            case 412: return "DAIREN";
            case 815: return "DARWIN";
            case 979: return "DOROTEA";
            case 577: return "DOVER";
            case 149: return "DULUTH";
            case 711: return "DURBAM";
            case 595: return "DUSSELDORF";
            case 717: return "EAST-LONDON";
            case 574: return "ETEN SALVERRY";
            case 142: return "EVERGLADES";
            case 135: return "FILADELFIA";
            case 594: return "FRANKFURT";
            case 812: return "FREMANTLE";
            case 449: return "FUKUYAMA";
            case 969: return "FUTALEUFU";
            case 156: return "GALVESTON";
            case 542: return "GENOVA";
            case 816: return "GERALDTON";
            case 604: return "GHENT";
            case 219: return "GOLFO DE MEXICO, OTROS NO ESPE";
            case 631: return "GOTEMBURGO";
            case 941: return "GREGORIO";
            case 937: return "GUARELLO";
            case 946: return "GUAYACAN";
            case 242: return "GUAYAQUIL";
            case 215: return "GUAYMAS";
            case 113: return "HALIFAX";
            case 592: return "HAMBURGO";
            case 126: return "HAMILTON";
            case 583: return "HANKO";
            case 634: return "HELSIMBORG";
            case 581: return "HELSINSKI";
            case 492: return "HONG KONG";
            case 159: return "HOUSTON";
            case 972: return "HUAHUM";
            case 920: return "HUASCO/GUACOLDA";
            case 565: return "HUELVA";
            case 976: return "HUEMULES";
            case 985: return "IBANEZ PALAVICINI";
            case 253: return "ILO";
            case 902: return "IQUIQUE";
            case 254: return "IQUITOS";
            case 929: return "ISLA DE PASCUA";
            case 143: return "JACKSONVILLE";
            case 922: return "JUAN FERNANDEZ";
            case 635: return "KALMAR";
            case 451: return "KAOHSIUNG";
            case 461: return "KARHG ISLAND";
            case 452: return "KEELUNG";
            case 584: return "KEMI";
            case 443: return "KOBE";
            case 585: return "KOKKOLA";
            case 586: return "KOTKA";
            case 282: return "LA GUAIRA";
            case 552: return "LA PALLICE";
            case 973: return "LAGO VERDE";
            case 553: return "LE HAVRE";
            case 928: return "LEBU";
            case 543: return "LIORNA, LIVORNO";
            case 909: return "LIRQUEN";
            case 611: return "LISBOA";
            case 571: return "LIVERPOOL";
            case 572: return "LONDRES";
            case 175: return "LONG BEACH";
            case 174: return "LOS ANGELES";
            case 965: return "LOS LIBERTADORES";
            case 199: return "LOS VILOS";
            case 927: return "LOTA";
            case 966: return "MAHUIL MALAL";
            case 633: return "MALMO";
            case 431: return "MANILA";
            case 217: return "MANZANILLO";
            case 269: return "MAR DEL PLATA";
            case 285: return "MARACAIBO";
            case 554: return "MARSELLA";
            case 216: return "MAZATLAN";
            case 915: return "MEJILLONES";
            case 264: return "MENDOZA";
            case 141: return "MIAMI";
            case 206: return "MICHILLA";
            case 150: return "MILWAUKEE";
            case 153: return "MOBILE";
            case 447: return "MOJI";
            case 981: return "MONTE AYMOND";
            case 272: return "MONTEVIDEO";
            case 111: return "MONTREAL";
            case 716: return "MOSSEL-BAY";
            case 445: return "NAGOYA";
            case 421: return "NANPO";
            case 544: return "NAPOLES";
            case 936: return "NATALES";
            case 263: return "NECOCHEA";
            case 132: return "NEW HAVEN";
            case 154: return "NEW ORLEANS";
            case 134: return "NEW YORK";
            case 137: return "NORFOLK";
            case 593: return "NUREMBERG";
            case 160: return "OAKLAND";
            case 645: return "ODENSE";
            case 599: return "OLDENBURG";
            case 961: return "OLLAGUE";
            case 605: return "OOSTENDE";
            case 442: return "OSAKA";
            case 651: return "OSLO";
            case 997: return "OTROS PTOS. CHILENOS";
            case 499: return "OTROS PUERTOS ASIATICOS NO ESP";
            case 799: return "OTROS PUERTOS DE AFRICA NO ESP";
            case 596: return "OTROS PUERTOS DE ALEMANIA NO E";
            case 399: return "OTROS PUERTOS DE AMERICA NO ES";
            case 261: return "OTROS PUERTOS DE ARGENTINA NO";
            case 813: return "OTROS PUERTOS DE AUSTRALIA NO";
            case 482: return "OTROS PUERTOS DE BANGLADESH NO";
            case 602: return "OTROS PUERTOS DE BELGICA NO ES";
            case 291: return "OTROS PUERTOS DE BRASIL NO ESP";
            case 522: return "OTROS PUERTOS DE BULGARIA NO E";
            case 117: return "OTROS PUERTOS DE CANADA NO IDE";
            case 413: return "OTROS PUERTOS DE CHINA NO ESPE";
            case 231: return "OTROS PUERTOS DE COLOMBIA NO E";
            case 423: return "OTROS PUERTOS DE COREA";
            case 537: return "OTROS PUERTOS DE CRO";
            case 643: return "OTROS PUERTOS DE DINAMARCA NO";
            case 241: return "OTROS PUERTOS DE ECUADOR NO ES";
            case 561: return "OTROS PUERTOS DE ESPANA NO ESP";
            case 180: return "OTROS PUERTOS DE ESTADOS UNIDO";
            case 699: return "OTROS PUERTOS DE EUROPA NO ESP";
            case 432: return "OTROS PUERTOS DE FILIPINAS NO";
            case 582: return "OTROS PUERTOS DE FINLANDIA NO";
            case 551: return "OTROS PUERTOS DE FRANCIA NO ES";
            case 623: return "OTROS PUERTOS DE HOLANDA NO ES";
            case 472: return "OTROS PUERTOS DE INDIA NO E";
            case 576: return "OTROS PUERTOS DE INGLATERRA NO";
            case 462: return "OTROS PUERTOS DE IRAN NO ESPEC";
            case 541: return "OTROS PUERTOS DE ITALIA NO ESP";
            case 441: return "OTROS PUERTOS DE JAPON NO ESPE";
            case 301: return "OTROS PUERTOS DE LAS ANTILLAS";
            case 210: return "OTROS PUERTOS DE MEXICO NO ESP";
            case 536: return "OTROS PUERTOS DE MON";
            case 652: return "OTROS PUERTOS DE NORUEGA NO ES";
            case 899: return "OTROS PUERTOS DE OCEANIA NO";
            case 224: return "OTROS PUERTOS DE PANAMA NO ESP";
            case 251: return "OTROS PUERTOS DE PERU NO ESPEC";
            case 612: return "OTROS PUERTOS DE PORTUGAL NO E";
            case 512: return "OTROS PUERTOS DE RUMANIA NO ES";
            case 534: return "OTROS PUERTOS DE SER";
            case 491: return "OTROS PUERTOS DE SINGAPURE NO";
            case 713: return "OTROS PUERTOS DE SUDAFRICA NO";
            case 632: return "OTROS PUERTOS DE SUECIA NO ESP";
            case 453: return "OTROS PUERTOS DE TAIWAN NO ESP";
            case 271: return "OTROS PUERTOS DE URUGUAY NO ES";
            case 281: return "OTROS PUERTOS DE VENEZUELA NO";
            case 532: return "OTROS PUERTOS DE YUGOESLAVIA N";
            case 587: return "OULO";
            case 202: return "OXIQUIM";
            case 970: return "PALENA-CARRENLEUFU";
            case 145: return "PALM BEACH";
            case 975: return "PAMPA ALTA";
            case 971: return "PANGUIPULLI";
            case 295: return "PARANAGUA";
            case 998: return "PASO JAMA";
            case 204: return "PATACHE";
            case 913: return "PATILLOS";
            case 925: return "PENCO";
            case 152: return "PENSACOLA";
            case 939: return "PERCY";
            case 968: return "PEREZ ROSALES";
            case 588: return "PIETARSAARI";
            case 949: return "PINO HACHADO(LIUCURA";
            case 148: return "PITTSBURGH";
            case 578: return "PLYMOUTH";
            case 535: return "PODGORITSA";
            case 589: return "PORI";
            case 155: return "PORT ARTHUR";
            case 120: return "PORT CARTIES";
            case 715: return "PORT-ELIZABETH";
            case 172: return "PORTLAND";
            case 208: return "POSEIDON";
            case 125: return "PRINCE RUPERT";
            case 201: return "PUCHOCO";
            case 207: return "PUERTO ANGAMOS";
            case 268: return "PUERTO MADRYN";
            case 910: return "PUERTO MONTT";
            case 943: return "PUERTO WILLIAMS";
            case 122: return "PUERTOS DEL GOLFO DE MEXICO, O";
            case 912: return "PUNTA ARENAS";
            case 947: return "PUNTA DELGADA";
            case 124: return "QUEBEC";
            case 933: return "QUELLON";
            case 921: return "QUINTERO";
            case 900: return "RANCHO DE NAVES Y AERONAVES DE";
            case 531: return "RIJEKA";
            case 538: return "RIJEKA";
            case 294: return "RIO GRANDE DEL SUR";
            case 293: return "RIO JANEIRO";
            case 954: return "RIO MAYER";
            case 955: return "RIO MOSCO";
            case 573: return "ROCHESTER";
            case 270: return "ROSARIO";
            case 598: return "ROSTOCK";
            case 622: return "ROTTERDAM";
            case 558: return "RUAN";
            case 115: return "SAINT JOHN";
            case 714: return "SALDANHA";
            case 545: return "SALERNO";
            case 945: return "SALINAS";
            case 297: return "SALVADOR";
            case 906: return "SAN ANTONIO";
            case 176: return "SAN DIEGO";
            case 173: return "SAN FRANCISCO";
            case 964: return "SAN FRANCISCO";
            case 962: return "SAN PEDRO DE ATACAMA";
            case 982: return "SAN SEBASTIAN";
            case 908: return "SAN VICENTE";
            case 292: return "SANTOS";
            case 296: return "SAO PAULO";
            case 140: return "SAVANAH";
            case 547: return "SAVONA";
            case 171: return "SEATLE";
            case 613: return "SETUBAL";
            case 566: return "SEVILLA";
            case 411: return "SHANGAI";
            case 446: return "SHIMIZUI";
            case 811: return "SIDNEY";
            case 963: return "SOCOMPA";
            case 653: return "STAVANGER";
            case 161: return "STOCKTON";
            case 203: return "T. GASERO ABASTIBLE";
            case 907: return "TALCAHUANO";
            case 916: return "TALTAL";
            case 151: return "TAMPA";
            case 211: return "TAMPICO";
            case 567: return "TARRAGONA";
            case 996: return "TERM. PETROLERO ENAP";
            case 944: return "TERRITORIO ANTARTICO CHILENO";
            case 914: return "TOCOPILLA";
            case 924: return "TOME";
            case 116: return "TORONTO";
            case 935: return "TORTEL";
            case 209: return "TRES PUENTES";
            case 984: return "TRIANA";
            case 905: return "VALPARAISO";
            case 114: return "VANCOUVER";
            case 521: return "VARNA";
            case 948: return "VENTANAS";
            case 213: return "VERACRUZ";
            case 986: return "VILLA OHIGGINS";
            case 956: return "VISVIRI";
            case 138: return "WILMINGTON";
            case 448: return "YAWATA";
            case 444: return "YOKOHAMA";
            case 603: return "ZEEBRUGGE";
            case 952: return "ZONA FRANCA IQUIQUE";
            case 953: return "ZONA FRANCA PUNTA ARENAS";
            default:
                return "Codigo Puerto";

        }
    }

    public static string NombreCodigoClausulaVenta(int _CodClausulaVenta)
    {
        switch (_CodClausulaVenta)
        {
            case 1:
                return "COSTOS, SEGURO Y FLETE (CIF)";
            case 2:
                return "COSTOS Y FLETE (CFR)";
            case 3:
                return "EN FÁBRICA (EXW)";
            case 4:
                return "FRANCO AL COSTADO DEL BUQUE (FAS)";
            case 5:
                return "FRANCO A BORDO (FOB)";
            case 6:
                return "SIN CLÁUSULA DE COMPRAVENTA (S/CL)";
            case 9:
                return "ENTREGADAS DERECHOS PAGADOS (DDP)";
            case 10:
                return "FRANCO TRANSPORTISTA (FCA)";
            case 11:
                return "TRANSPORTE PAGADO HASTA (CPT)";
            case 12:
                return "TRANSPORTE Y SEGURO PAGADO HASTA (CIP)";
            case 17:
                return "ENTREGADAS EN PUERTO DESTINO (DAT)";
            case 18:
                return "ENTREGADAS EN LUGAR CONVENIDO (DAP)";
            case 8:
                return "Otros";
            default:
                return "Codigo Clausula de Venta";
        }
    }

    public static string NombreCodigoViaTransporte(int _CodViaTransporte)
    {
        switch (_CodViaTransporte)
        {
            case 1:
                return "MARÍTIMA, FLUVIAL Y LACUSTRE";
            case 4:
                return "AÉREO";
            case 5:
                return "POSTAL";
            case 6:
                return "FERROVIARIO";
            case 7:
                return "CARRETERO / TERRESTRE";
            case 8:
                return "OLEODUCTOS, GASODUCTOS";
            case 9:
                return "TENDIDO ELÉCTRICO (Aéreo, Subterráneo)";
            case 10:
                return "OTRA";
            default:
                return "Codigo Via de Transporte";
        }
    }

    public static string NombreCodigoTipoBultos(int _CodViaTipoBultos)
    {
        switch (_CodViaTipoBultos)
        {
            case 26: return "ARMAZON";
            case 90: return "ATADO";
            case 86: return "ATAUD";
            case 85: return "AUTOMOTOR";
            case 65: return "BALA";
            case 27: return "BANDEJA";
            case 16: return "BARRA";
            case 44: return "BARRIL";
            case 37: return "BARRILETE";
            case 24: return "BAUL";
            case 34: return "BIDON";
            case 19: return "BLOQUE";
            case 91: return "BOBINA";
            case 64: return "BOLSA";
            case 32: return "BOTELLA";
            case 31: return "BOTELLAGAS";
            case 93: return "BULTONOESP";
            case 22: return "CAJA DE CARTON";
            case 29: return "CAJALATA";
            case 28: return "CAJAMADERA";
            case 40: return "CAJANOESP";
            case 21: return "CAJON";
            case 83: return "CARRETE";
            case 36: return "CESTA";
            case 12: return "CILINDRO";
            case 25: return "COFRE";
            case 73: return "CONT20";
            case 74: return "CONT40";
            case 78: return "CONTNOESP";
            case 51: return "CUBO";
            case 46: return "CUNETE";
            case 43: return "DAMAJUANA";
            case 77: return "ESTANQUE";
            case 23: return "FARDO";
            case 42: return "FRASCO";
            case 5: return "GAS";
            case 2: return "GRANOS";
            case 35: return "JABA";
            case 41: return "JARRO";
            case 33: return "JAULA";
            case 82: return "LAMINA";
            case 17: return "LINGOTE";
            case 4: return "LIQUIDO";
            case 63: return "MALETA";
            case 88: return "MAQUINARIA";
            case 3: return "NODULOS";
            case 80: return "PALLET";
            case 61: return "PAQUETE";
            case 10: return "PIEZA";
            case 39: return "PIPA";
            case 89: return "PLANCHA";
            case 1: return "POLVO";
            case 66: return "RED";
            case 75: return "REEFER20";
            case 76: return "REEFER40";
            case 20: return "ROLLIZO";
            case 13: return "ROLLO";
            case 99: return "S/EMBALAR";
            case 62: return "SACO";
            case 98: return "SIN BULTO";
            case 67: return "SOBRE";
            case 81: return "TABLERO";
            case 45: return "TAMBOR";
            case 47: return "TARRO";
            case 38: return "TONEL";
            case 18: return "TRONCO";
            case 11: return "TUBO";
            default:
                return "Codigo tipo bultos";
        }
    }

    public static string NombreCodigoFormaPago(int _CodFormaPago)
    {
        switch (_CodFormaPago)
        {
            case 01:
                return "Cobranza hasta 1 año";
            case 02:
                return "Cobranza más de 1 año";
            case 11:
                return "Acreditivo hasta 1 año";
            case 12:
                return "Crédito de bancos y Org Financieros más de 1 año";
            case 21:
                return "Sin Pago";
            case 32:
                return "pago Anticipado a la fecha de embarque";
            default:
                return "Forma de Pago";
        }
    }

    public static string NombreCodigoUnidadMedida(int _CodUnidadMedida)
    {
        switch (_CodUnidadMedida)
        {
            case 1: return "TMB";
            case 2: return "QMB";
            case 3: return "MKWH";
            case 4: return "TMN";
            case 5: return "KLT";
            case 6: return "KN";
            case 7: return "GN";
            case 8: return "HL";
            case 9: return "LT";
            case 10: return "U";
            case 11: return "DOC";
            case 12: return "U(JGO)";
            case 13: return "MU";
            case 14: return "MT";
            case 15: return "MT2";
            case 16: return "MCUB";
            case 17: return "PAR";
            case 18: return "KNFC";
            case 19: return "CARTON";
            case 20: return "KWH";
            case 23: return "BAR";
            case 24: return "M2/1MM";
            case 99: return "S.U.M";
            default:
                return "Unidad de Medida";
        }
    }
    

}
