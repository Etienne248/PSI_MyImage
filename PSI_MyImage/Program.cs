using System;

namespace PSI_TD2
{
    class Program
    {
        static void Main()
        {
            MyImage image = null;
            string s;
            do
            {
                Console.WriteLine("\nVoulez vous créer une image ou bien en modifier une préexistante? (taper 0 pour quitter)");
                Console.WriteLine("Vous pouvez rentrer:");
                Console.WriteLine("1-créer\n2-modifier\n3-ouvrir\n4-enregistrer");
                s = Console.ReadLine();
                if (s == "1" || s == "créer")
                {
                    Console.WriteLine();

                    Console.WriteLine("Entrez une méthode de création d'une nouvelle image ou 0 pour revenir en arrière (l'image selectionné sera créer pour laisser place à l'image créer!):");
                    Console.WriteLine("Vous pouvez rentrer par exemple:");
                    Console.WriteLine("1-mandelbrot\n2-histogramme\n3-coder\n4-décoder");
                    s = Console.ReadLine();
                    string m;
                    switch (s)
                    {
                        case "1":// il est necessaire d'avoir deja ouvert un image avec l'option "3" pour "histogramme" et "decoder"
                        case "mandelbrot":
                            Console.WriteLine("voulez-vous generez l'image par défaut ou la personnaliser?");
                            Console.WriteLine("choisissez la hauteur de l'image ( ecrivez 0 si vous voulez les valeurs par defaut) ");
                            m = Console.ReadLine();
                            if (m == "0")
                            {
                                image = MyImage.Dessin_Mandelbrot(2000, 4000, 0, 0, 0.001, 50);

                            }
                            else// les parametres personnalisés permettent de zoomer sur une partie de l'ensemble, pour cela il faut arriver à bien jouer 
                            //avec les parametres "hauteur" "largeur" et "distanceentrepixel" (+ ces parametres augmentent plus l'image sera dézoomer)
                            // vous pourriez toujours zoomer apres avoir generer l'image mais pour avoir un résultat satisfaisant, il faudra sans doute generez
                            //des centaines de millions de pixels...
                            {

                                int hauteur = Convert.ToInt32(m);
                                Console.WriteLine("choisissez la largeur de l'image ");
                                int largeur = Convert.ToInt32(Console.ReadLine());
                                Console.WriteLine("choisissez l'ordonnée du centre de l'image  ");
                                double ordonneeMilieu = Convert.ToDouble(Console.ReadLine());
                                Console.WriteLine("choisissez l'abcisse du centre de l'image ");
                                double abscisseMilieu = Convert.ToDouble(Console.ReadLine());
                                Console.WriteLine("choisissez la distance Entre chaque Pixel ");
                                double distanceEntrePixel = Convert.ToDouble(Console.ReadLine());
                                Console.WriteLine("choisissez le nombre d'iteration max ");
                                int iteration_max = Convert.ToInt32(Console.ReadLine());
                                image = MyImage.Dessin_Mandelbrot(hauteur, largeur, ordonneeMilieu, abscisseMilieu, distanceEntrePixel, iteration_max);

                            }

                            break;
                        case "2":
                        case "histogramme":
                            if (image == null) Console.WriteLine("vous devez avant cela ouvrir une image avec l'option 3 ! ");
                            else
                            {
                                Console.WriteLine("choisissez la hauteur de votre histogramme ");
                                int hauteur = Convert.ToInt32(Console.ReadLine());
                                image = image.Histogramme(hauteur);
                            }
                            break;
                        case "3":
                        case "coder":
                            Console.WriteLine("inscrivez le nom de l'image qui cache ");
                            image = new MyImage(Console.ReadLine() + ".bmp");
                            Console.WriteLine("inscrivez le nom de l'image à caché ");
                            MyImage image2 = new MyImage(Console.ReadLine() + ".bmp");
                            Console.WriteLine("quelle est l'abcisse du pixel en haut a gauche de l'image à cacher dans l'image qui cache? Attention à ce que l'image a cacher rentre bien!");
                            int abcisse = Convert.ToInt32(Console.ReadLine());
                            Console.WriteLine("quelle est l'ordonné du pixel en haut a gauche de l'image à cacher dans l'image qui cache?  Attention à ce que l'image a cacher rentre bien!");
                            int ordonné = Convert.ToInt32(Console.ReadLine());
                            image.Codage(image2, abcisse, ordonné);
                            break;
                        case "4":
                        case "décoder":
                            if (image == null) Console.WriteLine("vous devez avant cela ouvrir une image avec l'option 3 ! ");

                            else
                            {
                                Console.WriteLine("Donner l'abcisse du pixel en haut a gauche de l'image caché (taper -1 un pour analyser toute l'image)");
                                m = Console.ReadLine();
                                if (m == "-1")
                                {
                                    image = image.Décodage(0, 0, image.Image.GetLength(0), image.Image.GetLength(1));

                                }
                                else
                                {
                                    int abcisse2 = Convert.ToInt32(m);
                                    Console.WriteLine("Donner l'ordonné du pixel en haut a gauche de l'image caché  ");
                                    int ordonné2 = Convert.ToInt32(Console.ReadLine());
                                    Console.WriteLine("quelle est la hauteur de l'image caché?");
                                    int hauteur_caché = Convert.ToInt32(Console.ReadLine());
                                    Console.WriteLine("quelle est la longueur de l'image caché?");
                                    int largeur_caché = Convert.ToInt32(Console.ReadLine());
                                    image = image.Décodage(abcisse2, ordonné2, hauteur_caché, largeur_caché);
                                }
                            }
                            break;

                        case "5":
                        case "coder_QR":
                            break;
                        case "6":
                        case "décoder_QR":
                            break;

                    }
                }
                else if (s == "modifier" || s == "2")
                {
                    Console.WriteLine();

                    if (image == null) Console.WriteLine("Vous devez d'abord ouvrir ou créer une image");
                    else
                    {

                        Console.WriteLine("Entrez une méthode de traitement ou 0 pour revenir en arrière:");
                        Console.WriteLine("Vous pouvez rentrer par exemple:");
                        Console.WriteLine("1-rotation\n2-redimensionnement\n3-miroir\n4-noir_et_blanc\n5-nuance_de_gris\n6-détection_de_contour\n7-renforcement_des_bords\n8-flou\n9-repoussage");
                        s = Console.ReadLine();
                        switch (s)
                        {
                            case "1":
                            case "rotation":
                                Console.WriteLine("choisissez un angle entier positif");
                                int angle = Convert.ToInt32(Console.ReadLine());
                                image.Rotation(angle);
                                break;
                            case "2":
                            case "redimensionnement":
                                Console.WriteLine("choisissez un facteur positif");
                                int facteur = Convert.ToInt32(Console.ReadLine());
                                image.Agrandir_Retrecir(facteur);
                                break;
                            case "3":
                            case "miroir":
                                image.Miroir();
                                break;
                            case "4":
                            case "noir_et_blanc":
                                Console.WriteLine("choisissez une valeur entre 0 et 255, plus la valeur est proche de 0 plus l'image sera  blanche");
                                int facteur2 = Convert.ToInt32(Console.ReadLine());
                                image.Noir_Et_Blanc(facteur2);
                                break;
                            case "5":
                            case "nuance_de_gris":
                                image.Nuance_De_Gris();
                                break;
                            case "6":
                            case "détection_de_contour":
                                image.Détection_de_contour();
                                break;
                            case "7":
                            case "renforcement_des_bords":
                                image.Renforcement_des_bords();
                                break;
                            case "8":
                            case "flou":
                                image.Flou();
                                break;
                            case "9":
                            case "repoussage":
                                image.Repoussage();
                                break;
                        }
                        Console.WriteLine(" \n\nVous pouvez cumuler les traitements si vous le souhaitez\n");// attention ce n'est pas encore très optimisé
                                                                                                             // on risque d'avoir des résultats qui ne conviennent pas ( rotation 45 + flou par ex)
                    }
                }

                else if (s == "3" || s == "ouvrir")
                {
                    Console.WriteLine("donner le nom de l'image que vous souhaitez ouvrir. N'écrivez pas de .bmp, et assurez vous que l'image se trouve dans le dossier le plus bas des fichiers du code (exemple:coco)");
                    image = new MyImage(Console.ReadLine() + ".bmp");// créer une instance de MyImage a partir de ce fichier
                    Console.WriteLine();
                }
                else if (s == "4" || s == "enregistrer")
                {
                    if (image == null) Console.WriteLine("\nVous n'avez même pas ouvert d'image!");
                    else
                    {
                        Console.WriteLine("donner le nom sur lequel vous voulez enregistrer l'image (un nouveau .bmp à ce nom sera créer)");
                        image.From_Image_To_File(Console.ReadLine() + ".bmp");
                    }
                }

            } while (s != "0");

        }

        #region fonction supplémentaire

        public static void Affiche<T>(T[] tableau)// fonction qui affiche  l'image quand elle est sous forme de tableau ( l'affiche n'est pas très propre car on ne saute pas de ligne,
                                                  // sons seul interet est de verifier que l'image de depart est bien la meme, meme apres avoir été converti dans la classe MyImage
        {
            if (tableau == null)
            {
                Console.WriteLine("tableau null");
            }

            else
            {
                if (tableau.Length == 0)
                {
                    Console.WriteLine("tableau vide");
                }
                else
                {
                    for (int i = 0; i < tableau.GetLength(0); i++)
                    {
                        Console.Write(tableau[i].ToString() + " ");
                    }
                }
            }
        }
        public static void Affiche<T>(T[,] matrice)//fonction qui affiche  l'image quand elle est sous forme de matrice
        {
            if (matrice == null)
            {
                Console.WriteLine("matrice null");
            }

            else
            {
                if (matrice.Length == 0)
                {
                    Console.WriteLine("matrice vide");
                }
                else
                {
                    for (int i = 0; i < matrice.GetLength(0); i++)
                    {
                        for (int j = 0; j < matrice.GetLength(1); j++)
                        {
                            Console.Write(matrice[i, j].ToString() + " ");
                        }
                        Console.WriteLine();
                    }
                }
            }
        }
        #endregion fonction supplémentaire
    }
}
