using ReedSolomon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace Projet_PSI
{
    class MyImage
    {
        // toutes les propriétés importantes de l'image
        private string format;
        private int taille_fichier;
        private int taille_offset;
        private int largeur;
        private int hauteur;
        private int nbre_bits_par_couleur;
        private Pixel[,] image;// tableau de pixel


        #region TD2

        public Pixel[,] Image// propriété accesible pour la classe Program
        {
            get { return image; }
        }

        public MyImage(string myfile)// lit un fichier( .bmp) et le transforme en instance de la classe MyImage
        {
            // toutes les informations du header sont récupéré dans les attributs ( à noter que la fonction ne fonctionne que si l'offset à une taillle de 54)

            byte[] b = File.ReadAllBytes(myfile);// récupere toutes les données dans un tableau
            format = Encoding.UTF8.GetString(SubArray(b, 0, 2));// traduit les données depui le code ASCII ( 66 77 --> BM)
            taille_fichier = Convertir_Endian_To_Int(SubArray(b, 2, 4));// toutes les données sont en endian,on les transforme donc en int
            taille_offset = Convertir_Endian_To_Int(SubArray(b, 10, 4));
            largeur = Convertir_Endian_To_Int(SubArray(b, 18, 4));
            hauteur = Convertir_Endian_To_Int(SubArray(b, 22, 4));
            nbre_bits_par_couleur = Convertir_Endian_To_Int(SubArray(b, 28, 2));

            image = new Pixel[hauteur, largeur];// création d'un tableau de Pixel
            int n = taille_offset;// n augmente  de 1 à chaque nouvelle affectation dans chaque pixel
            int espaceVide = (4 - ((largeur * 3) % 4)) % 4;//on prend en compte le cas où l'image n'est pas un multiple de 4 en sautant les zéros en plus
            for (int i = hauteur - 1; i >= 0; i--)// on commence en bas a gauche et on remonte les lignes durant le balayage
            {
                for (int j = 0; j < largeur; j++)
                {
                    byte B = b[n];
                    n++;
                    byte G = b[n];
                    n++;
                    byte R = b[n];
                    n++;
                    image[i, j] = new Pixel(R, G, B);
                }
                n += espaceVide;
            }
        }

        public byte[] From_Image_To_File(string file) //prend une instance de MyImage et la transforme en fichier binaire respectant la structure du fichier.bmp
        {
            int espaceVide = (4 - ((largeur * 3) % 4)) % 4; ;//on prend en compte le cas où l'image n'est pas un multiple de 4 en mettant les zéros en plus

            byte[] b = new byte[taille_offset + hauteur * (largeur * (nbre_bits_par_couleur / 8) + espaceVide)];
            // taille de l'offset + taille de l'image (en comptant le contenue de chaque pixel) + espaceVide*hauteur

            //copie tout les attributs de MyImage dans le bonne ordre pour recréer le header de BitMap
            Copy(Encoding.UTF8.GetBytes(format), b, 0);// transforme le format du fichier en code ASCII ( BM --> 66 77 )
            Copy(Convertir_Int_To_Endian(taille_fichier, 4), b, 2);

            Copy(Convertir_Int_To_Endian(taille_offset, 4), b, 10);
            Copy(Convertir_Int_To_Endian(taille_offset - 14, 4), b, 14);
            Copy(Convertir_Int_To_Endian(largeur, 4), b, 18);
            Copy(Convertir_Int_To_Endian(hauteur, 4), b, 22);

            b[26] = 1;
            b[27] = 0;

            Copy(Convertir_Int_To_Endian(nbre_bits_par_couleur, 2), b, 28);
            Copy(Convertir_Int_To_Endian(taille_fichier - taille_offset, 4), b, 34);

            //balaye l'image et transfere toute les données dans un tableau
            int n = taille_offset;
            for (int i = hauteur - 1; i >= 0; i--)
            {
                for (int j = 0; j < largeur; j++)
                {
                    if (image[i, j] != null)
                    {
                        b[n] = (byte)image[i, j].Bleu;
                        n++;
                        b[n] = (byte)image[i, j].Vert;
                        n++;
                        b[n] = (byte)image[i, j].Rouge;
                        n++;
                    }
                    else n += 3;
                }
                n += espaceVide;
            }
            File.WriteAllBytes(file, b);// réecrit ce tableau dansn un fichier au bon format
            return b; // on recupere le tableau juste pour verifier que tout fonctionne
        }


        public static int Convertir_Endian_To_Int(byte[] bytes) //convertit une séquence d’octets au format little endian en entier
        {
            int result = 0;

            for (int i = 0; i < bytes.Length; i++)
            {
                result |= bytes[i] << i * 8;
            }
            return result;

        }

        public static byte[] Convertir_Int_To_Endian(int val, int nombreOctet) //convertit un entier en séquence d’octets au format little endian
        {
            byte[] endian = new byte[nombreOctet];
            for (int i = 0; i < nombreOctet; i++)
            {
                endian[i] |= (byte)(val >> 8 * i);
            }
            return endian;
        }


        public string HeaderToString()// permet d'afficher le header dans la classe MyImage
        {
            return format + " " + taille_fichier + " " + taille_offset + " " + largeur + " " + hauteur + " " + nbre_bits_par_couleur;
        }

        public static T[] SubArray<T>(T[] array, int offset, int length)// recupere un sous tableau et le copie dans un autre tableau
        {
            T[] result = new T[length];
            Array.Copy(array, offset, result, 0, length);
            return result;
        }
        public static void Copy<T>(T[] arrayCopy, T[] array, int offset)//fonction Array.Copy mais avec des paramètres prédéfini.
        {
            Array.Copy(arrayCopy, 0, array, offset, arrayCopy.Length);
        }

        public static int Pow(int a, int b)// fonction puissance utile pour la conversion d'endian à int
        {
            int r = 1;
            for (int i = 0; i < b; i++) r *= a;
            return r;
        }
        #endregion TD2

        #region TD3
        public void Miroir()// fonction qui renvoie le miroir de l'image 
        {
            Pixel p;
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur / 2; j++)// on s'arrete a la moitié de la largeur, sinon lorsque la boucle aura depassé la moitié, les pixels reviendront a leurs places d'origine
                {
                    p = image[i, j];// nouvelle instance de pixel
                    image[i, j] = image[i, largeur - j - 1];// on change les pixels de place selon un axe de symetrie verticale 
                    image[i, largeur - j - 1] = p;
                }
            }

        }

        public void Nuance_De_Gris()
        {
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    if (image[i, j].Rouge != image[i, j].Vert || image[i, j].Rouge != image[i, j].Bleu || image[i, j].Vert != image[i, j].Bleu)
                    {
                        int moyenne = (image[i, j].Rouge + image[i, j].Vert + image[i, j].Bleu) / 3;// chaque pixel aura la valeur moyenne prise par l'ensemble de valeurs initials de ses couleurs
                        image[i, j].Rouge = (byte)moyenne;
                        image[i, j].Vert = (byte)moyenne;
                        image[i, j].Bleu = (byte)moyenne;
                    }
                }
            }
        }

        public void Noir_Et_Blanc(int n)
        {
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    int moyenne = (image[i, j].Rouge + image[i, j].Vert + image[i, j].Bleu) / 3;// si la moyenne de la valeur des couleurs est... 
                    if (moyenne < n) image[i, j] = new Pixel(0, 0, 0); //...inférieur à n le pixel sera noir
                    else image[i, j] = new Pixel(255, 255, 255);//... supérieur à n le pixel sera blanc
                }
            }
        }

        public void Agrandir_Retrecir(float facteur)
        {
            largeur = (int)(largeur * facteur);
            hauteur = (int)(hauteur * facteur);
            Pixel[,] m = new Pixel[hauteur, largeur];// on creer une nouvelle instance d'image avec les bonnes proportions 
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    m[i, j] = image[(int)(i / facteur), (int)(j / facteur)];//on replace chaque pixel de l'ancienne image  dans l'image redimensionner en fonction du facteur
                }
            }
            image = m;// on met la nouvelle image comme attribut de l'objet
        }

        public void Rotation(int angle)
        {
            // on decompose chaque angle en X*nb90deg +angleautre
            int nb90deg = (angle / 90) % 4;
            int angleAutre = angle % 90;
            Rotation90(nb90deg); //rotation en angle droit
            if (angleAutre != 0) Rotation0_90(angleAutre); // rotation de 0 à 90 degré
        }

        public void Rotation0_90(int angle)// (rotation horaire) le programme ne fonctionne que pour des angle <=90
        {
            double angleRad = angle * Math.PI / 180; //conversion des degré en radian
            double c = Math.Cos(angleRad);  //calcule du cosinus de l'angle
            double s = Math.Sin(angleRad);  //calcule du sinus de l'angle
            int l = (int)(c * largeur + s * hauteur);   //calcule de la nouvelle largeur
            int h = (int)(s * largeur + c * hauteur);   //calcule de la nouvelle hauteur
            Pixel[,] m = new Pixel[h, l];    //création d'une nouvelle matrice de dimention de l'image que l'on veut obtenir

            //on parcour tout les pixel de l'image d'origine
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    m[(int)(j * s + i * c), (int)(j * c + (hauteur - 1 - i) * s)] = image[i, j];//on place le pixel dans son nouvel emplacement dans la nouvelle image
                    //cette fonction crée des trous dans l'image que l'on peut boucher en fesant une interpolation.
                }
            }
            //changement de la hauteur, la largueur et de l'image vers les nouvelle valeur
            hauteur = h;
            largeur = l;
            image = m;
        }
        public void Rotation90(int nb90deg) // fait la rotation pour des angle à 90, 180 et 270 degré dans le sens des aiguilles d'une montre
        {
            if (nb90deg != 0)
            {
                Pixel[,] m;
                if (nb90deg == 1 || nb90deg == 3) // cas pour 90 ou 270 degré
                {
                    m = new Pixel[largeur, hauteur]; // hauteur et largeur inversé
                    if (nb90deg == 1) // rotation de 90 degré
                    {
                        for (int i = 0; i < hauteur; i++)
                        {
                            for (int j = 0; j < largeur; j++)
                            {
                                m[j, hauteur - 1 - i] = image[i, j];
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < hauteur; i++)// rotation de 270 degré (-90 degré)
                        {
                            for (int j = 0; j < largeur; j++)
                            {
                                m[largeur - 1 - j, i] = image[i, j];
                            }
                        }
                    }
                    int n = hauteur;
                    hauteur = largeur;
                    largeur = n;
                }
                else
                {
                    m = new Pixel[hauteur, largeur];
                    for (int i = 0; i < hauteur; i++)// rotation de 180 degré
                    {
                        for (int j = 0; j < largeur; j++)
                        {
                            m[hauteur - 1 - i, largeur - 1 - j] = image[i, j];
                        }
                    }
                }
                image = m;
            }
        }
        #endregion TD3

        #region TD4

        public Pixel Convolution(double[,] convolution, int indL, int indC)// fonction generique pour la convolution
        // que l'on applique aux coordonnés (indL, indC) de l'image
        {
            // nouvelle valeurs des pixels
            double newRouge = 0;
            double newVert = 0;
            double newBleu = 0;

            for (int i = 0; i <= 2; i++)// on balaye le voisinage du pixel
            {
                for (int j = 0; j <= 2; j++)
                {
                    if (indL - 1 + i >= 0 && indL - 1 + i < hauteur && indC - 1 + j >= 0 && indC - 1 + j < largeur && image[i, j] != null)
                    // pour verifier que chque voisinage est bien compris dans l'image
                    {
                        // on applique la convolution pour chaque valeurs des pixels
                        newRouge += convolution[i, j] * image[indL - 1 + i, indC - 1 + j].Rouge;
                        newVert += convolution[i, j] * image[indL - 1 + i, indC - 1 + j].Vert;
                        newBleu += convolution[i, j] * image[indL - 1 + i, indC - 1 + j].Bleu;
                    }
                }
            }
            return new Pixel((int)(newRouge), (int)(newVert), (int)(newBleu));// et on retourne le nouveau pixel
            // arrondie à l'entier
        }
        public void ConvolutionImageComplete(double[,] convolution)// balaye toute l'image pour lui appliquer 
                                                                   //la convolution a chaque pixel
        {
            Pixel[,] imagebis = new Pixel[hauteur, largeur];// creation d'une nouvelle image
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    imagebis[i, j] = Convolution(convolution, i, j);// on applique la convolution a chaque pixel
                }

            }
            this.image = imagebis;
        }
        public void Détection_de_contour()// detecte les contours et efface le reste
        {
            ConvolutionImageComplete(new double[,] { { -1, -1, -1 }, { -1, 8, -1 }, { -1, -1, -1 } });
        }
        public void Renforcement_des_bords()// recréer chaque pixel uniquement en fonction de celui de gauche
        {
            ConvolutionImageComplete(new double[,] { { 0, 0, 0 }, { -1, 1, 0 }, { 0, 0, 0 } });
        }
        public void Flou()// créer un flou
        {
            ConvolutionImageComplete(new double[,] { { 1.0 / 9, 1.0 / 9, 1.0 / 9 }, { 1.0 / 9, 1.0 / 9, 1.0 / 9 }, { 1.0 / 9, 1.0 / 9, 1.0 / 9 } });
        }
        public void Repoussage()// met en valeur les voisinages situés en bas a gauche et limite l'influence
                                // de ceux en haut a droite
        {
            ConvolutionImageComplete(new double[,] { { -2, -1, 0 }, { -1, 1, 1 }, { 0, 1, 2 } });
        }


        #endregion TD4

        #region TD5

        public MyImage(int hauteur, int largeur) // creation de class MyImage avec seulement la hauteur et la largeur
        {

            this.hauteur = hauteur;
            this.largeur = largeur;
            image = new Pixel[hauteur, largeur];
            format = "BM";
            taille_offset = 54;
            taille_fichier = taille_offset + hauteur * largeur;
            nbre_bits_par_couleur = 24;
        }

        public static MyImage Dessin_Mandelbrot(int hauteur, int largeur, double ordonneeMilieu, double abscisseMilieu, double distanceEntrePixel, int iteration_max)
        {
            MyImage image = new MyImage(hauteur, largeur);//création de l'image vierge
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    // on utilise la class Complex
                    Complex suite = new Complex(0, 0);  //premier terme de la suite
                    // "constante" prend les cordonnées du pixel en base complexe
                    Complex constante = new Complex((j - largeur / 2) * distanceEntrePixel + abscisseMilieu, (i - hauteur / 2) * distanceEntrePixel - ordonneeMilieu);

                    //on test si la suit z=z^2+c converge : si la suit dépassé 2 durant "iteration_max" itération alors on considere qu'elle diverge
                    for (int w = 0; w < iteration_max && suite.Magnitude < 2; w++)
                    {
                        suite = suite * suite + constante;
                    }
                    if (suite.Magnitude < 2)
                    {
                        image.Image[i, j] = new Pixel(0, 0, 0); //si la suite n'a pas dépassé 2 alors on met le pixel en noir
                    }
                    else { image.Image[i, j] = new Pixel(255, 255, 255); } //sinon on le met en blanc

                }
            }
            return image;//return l'image
        }

        public MyImage Histogramme(int hauteur_histo)
        {
            int[,] stat = new int[3, 256];// matrice contenant le nombre de pixel contenant chaque niveau de couleur primaire rouge vert et bleu (une couleur par ligne)
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    stat[0, image[i, j].Rouge]++;//on incrémente la couleur rouge correspondante 
                    stat[1, image[i, j].Vert]++;//on incrémente la couleur vert correspondante 
                    stat[2, image[i, j].Bleu]++;//on incrémente la couleur bleu correspondante 
                }
            }
            MyImage image_histo = new MyImage(hauteur_histo, 256);//création de l'image de l'histogramme vierge

            for (int j = 0; j < 256; j++)//on complète colonne par colonne l'histogramme
            {
                List<int> indexTrier = triIndex(column(stat, j));//tri en ordre croissant les valeur du rouge, vert et bleu; on obtion une liste des index
                for (byte n = 0; n < 3; n++)//on conplete l'histogramme pour chaque couleur rouge, vert et bleu dans l'ordre croissant des valeurs
                {
                    Pixel couleur = indexTrier[n] == 0 ? new Pixel(255, 0, 0) : indexTrier[n] == 1 ? new Pixel(0, 255, 0) : new Pixel(0, 0, 255);//test quelle couleur mettre
                    //on remplie les colonne avec la premier couleur du bas jusqu'à sa valeur puis la deuxième couleur depuit la valeur de la dernière couleur jusqu'a sa valeur et de même pour la couleur suivante
                    // de cette maniere on evite que la synthese additive ne fasse apparaitre d'autre couleurs aux endroit où, disons le rouge et le vert se superposent, ce qui rendrait le graphe illisible
                    for (int i = hauteur_histo > stat[indexTrier[n], j] ? hauteur_histo - stat[indexTrier[n], j] - 1 : 0; n == 0 ? i < hauteur_histo : i < hauteur_histo - stat[indexTrier[n - 1], j] - 1; i++)
                    {
                        image_histo.Image[i, j] = couleur;
                    }
                }
                for (int i = 0; i < hauteur_histo - stat[indexTrier[2], j]; i++)
                {
                    image_histo.Image[i, j] = new Pixel(255, 255, 255);//le reste est remplis avec du blanc
                }
            }
            return image_histo;//renvoie l'histogramme
        }

        public static List<int> triIndex(int[] tab)//tri les index par ordre croissant des valeur corespondente
        {
            List<int> indexTrier = new List<int>();//liste des index trié
            for (int i = 0; i < tab.Length; i++)
            {
                int x = 0;
                while (x < i && tab[i] > tab[indexTrier[x]]) x++;
                indexTrier.Insert(x, i);
            }
            return indexTrier;
        }

        public static T[] column<T>(T[,] matrice, int colonne)//récupère une colonne d'une matrice
        {
            int l = matrice.GetLength(0);
            T[] columnArray = new T[l];
            for (int i = 0; i < l; i++)
            {
                columnArray[i] = matrice[i, colonne];
            }
            return columnArray;
        }

        public void Codage(MyImage image_a_cacher, int abcisse, int ordonné)// methode statique qui prend deux images et qui cache l'une d'entre elles
                                                                            // à partir du pixel de coordonné (abcisse,ordonné) de l'image qui cache ( qui correspond alors aux coordonnées du pixel en haut à gauche de l'image à caché)
        {
            if (image_a_cacher.hauteur + ordonné > hauteur || image_a_cacher.largeur + abcisse > largeur)// on verifie que la méthode peut s'appliquer avec les 
                                                                                                         // parametres rentrés par l'utilisateur
            {
                Console.WriteLine("vous ne pouvez pas faire rentrer cette image dans les dimensions de l'autre !");
            }
            else
            {
                for (int i = 0; i < image_a_cacher.hauteur; i++)
                {
                    for (int j = 0; j < image_a_cacher.largeur; j++)
                    {
                        int i2 = i + ordonné;
                        int j2 = j + abcisse;
                        byte newRouge = (byte)((image[i2, j2].Rouge & 0b11110000) | image_a_cacher.Image[i, j].Rouge >> 4);
                        byte newVert = (byte)((image[i2, j2].Vert & 0b11110000) | image_a_cacher.Image[i, j].Vert >> 4);
                        byte newBleu = (byte)((image[i2, j2].Bleu & 0b11110000) | image_a_cacher.Image[i, j].Bleu >> 4);

                        image[i2, j2] = new Pixel(newRouge, newVert, newBleu);


                    }
                }
            }
        }

        /// <summary>
        /// il est necessaire pour decoder une image avec cette fonction de connaitre la position (abcisse,ordonné) du pixel 
        /// en haut à gauche de l'image cachée dans l'image qui cache, ainsi que ses dimensions
        /// </summary>
        /// <param name="abcisse"></param>
        /// <param name="ordonné"></param>
        /// <param name="hauteur_caché"></param>
        /// <param name="largeur_caché"></param>
        /// <returns></returns>
        public MyImage Décodage(int abcisse, int ordonné, int hauteur_caché, int largeur_caché)
        {
            MyImage image_caché = new MyImage(hauteur_caché, largeur_caché);// création d'une image aux dimensions de l'image cachée
            for (int i = 0; i < (hauteur_caché + ordonné <= hauteur ? hauteur_caché : hauteur - ordonné); i++)
            {
                for (int j = 0; j < (largeur_caché + abcisse <= largeur ? largeur_caché : largeur - abcisse); j++)
                {

                    int i2 = i + ordonné;
                    int j2 = j + abcisse;
                    byte newRouge = (byte)(image[i2, j2].Rouge << 4);
                    byte newVert = (byte)(image[i2, j2].Vert << 4);
                    byte newBleu = (byte)(image[i2, j2].Bleu << 4);

                    image_caché.Image[i, j] = new Pixel(newRouge, newVert, newBleu);// on créer un nouveau pixel de l'image caché avec ces informations

                }

            }
            return image_caché;
        }

        #endregion TD5

        #region TD6

        public static MyImage Codage_QR(string texte, int taille)
        {
            int nombreOctetsDonnees;
            int nombreOctetsEC;
            bool[,] QRcode;
            if (texte.Length == 0)
            {
                Console.WriteLine("texte nulle");
                return null;
            }
            else if (texte.Length < 26)
            {
                QRcode = new bool[21, 21];
                nombreOctetsDonnees = 19;
                nombreOctetsEC = 7;
            }
            else if (texte.Length < 48)
            {
                QRcode = new bool[25, 25];
                nombreOctetsDonnees = 34;
                nombreOctetsEC = 10;
            }
            else
            {
                Console.WriteLine("nous ne pouvons pas encoder ce texte, il y a trop de caractères");
                return null;
            }

            List<bool> donnees_et_EC = new List<bool>();
            donnees_et_EC.AddRange(new List<bool>() { false, false, true, false });
            donnees_et_EC.AddRange(Int_To_Bits(texte.Length, 9));
            byte[] alphanum = String_To_Alphanumerique(texte);
            for (int i = 0; i < alphanum.Length - 1; i += 2)
            {
                donnees_et_EC.AddRange(Int_To_Bits(alphanum[i] * 45 + alphanum[i + 1], 11));
            }
            if (texte.Length % 2 == 1) donnees_et_EC.AddRange(Int_To_Bits(alphanum[alphanum.Length - 1], 6)); ;

            for (int i = 0; i < donnees_et_EC.Count % 8; i++) donnees_et_EC.Add(false);

            int nombreOctetsEnPlus = nombreOctetsDonnees - (donnees_et_EC.Count / 8);
            for (int i = 0; i < nombreOctetsEnPlus; i++)
            {
                if (i % 2 == 0) donnees_et_EC.AddRange(Int_To_Bits(0b11101100, 8));
                else donnees_et_EC.AddRange(Int_To_Bits(0b00010001, 8));
            }
            byte[] EC = ReedSolomonAlgorithm.Encode(Bits_To_Bytes(donnees_et_EC), nombreOctetsEC, ErrorCorrectionCodeType.QRCode);
            donnees_et_EC.AddRange(Bytes_To_Bits(EC));
            ParcourirEmplacementDonnees(donnees_et_EC, QRcode, true);

            bool[] remplisage = new bool[] { true, true, false, true, false };
            MultiSquare(QRcode, remplisage, 3, 3);
            MultiSquare(QRcode, remplisage, 3, QRcode.GetLength(1) - 4);
            MultiSquare(QRcode, remplisage, QRcode.GetLength(0) - 4, 3);
            if (QRcode.GetLength(0) == 25) MultiSquare(QRcode, new bool[] { true, false, true }, 18, 18);

            List<bool> masque0correctionL = Int_To_Bits(0b111011111000100, 15);
            bool b;
            for (int i = 0; i < 15; i++)
            {
                b = masque0correctionL[i];
                if (i <= 6)
                {
                    QRcode[QRcode.GetLength(0) - 1 - i, 8] = b;
                    QRcode[8, i <= 5 ? i : 7] = b;
                }
                else
                {
                    QRcode[8, QRcode.GetLength(0) - 15 + i] = b;
                    QRcode[(i <= 8) ? (15 - i) : (14 - i), 8] = b;
                }
            }
            QRcode[QRcode.GetLength(0) - 8, 8] = true;

            b = true;
            for (int i = 8; i < QRcode.GetLength(0) - 8; i++)
            {
                QRcode[i, 6] = b;
                QRcode[6, i] = b;
                b = !b;
            }

            MyImage image = new MyImage(QRcode.GetLength(0) * taille, QRcode.GetLength(0) * taille);
            WriteBoolImage(QRcode, image, true);

            return image;
        }

        public static void ParcourirEmplacementDonnees(List<bool> donnees, bool[,] QRcode, bool WriteElseRead)
        {
            int i = 0;
            bool upElsedown = true;
            bool mode1 = QRcode.GetLength(0) == 21;
            for (int x = QRcode.GetLength(1) - 1; x >= 0; x -= 2)
            {
                if (x == 6) x--;
                int a = QRcode.GetLength(0) - (x > 8 ? 1 : 9);
                int b = (x > 8 && x < QRcode.GetLength(1) - 8 ? 0 : 9);
                for (int y = upElsedown ? a : b; upElsedown ? y >= b : y <= a; y += upElsedown ? -1 : 1)
                {
                    for (int xbis = x; xbis > x - 2 && i < donnees.Count; xbis--)
                    {
                        if ((mode1 || y > 18 + 2 || y < 18 - 2 || xbis > 18 + 2 || xbis < 18 - 2) && y != 6)
                        {
                            if (WriteElseRead) QRcode[y, xbis] = donnees[i];
                            else donnees[i] = QRcode[y, xbis];
                            i++;
                        }
                    }
                }
                upElsedown = !upElsedown;
            }
        }

        public static byte[] String_To_Alphanumerique(string texte)
        {
            byte[] alphanum = new byte[texte.Length];
            for (int i = 0; i < texte.Length; i++)
            {
                if (texte[i] >= 48 && texte[i] <= 57) alphanum[i] = (byte)(texte[i] - 48);
                else if (texte[i] >= 65 && texte[i] <= 90) alphanum[i] = (byte)(texte[i] - 55);
                else if (texte[i] == '$') alphanum[i] = 37;
                else if (texte[i] == '%') alphanum[i] = 38;
                else if (texte[i] == '*') alphanum[i] = 39;
                else if (texte[i] == '+') alphanum[i] = 40;
                else if (texte[i] == '-') alphanum[i] = 41;
                else if (texte[i] == '.') alphanum[i] = 42;
                else if (texte[i] == '/') alphanum[i] = 43;
                else if (texte[i] == ':') alphanum[i] = 44;
            }
            return alphanum;
        }

        public static void WriteBoolImage(bool[,] boolIm, MyImage image, bool boolImToImage)
        {
            Pixel noir = new Pixel(0, 0, 0);
            Pixel blanc = new Pixel(255, 255, 255);
            int facteur = image.hauteur / boolIm.GetLength(0);
            for (int i = image.hauteur - 1; i >= 0; i--)
            {
                for (int j = 0; j < image.largeur; j++)
                {
                    if (boolImToImage) image.image[i, j] = boolIm[i / facteur, j / facteur] ? noir : blanc;
                    else boolIm[i / facteur, j / facteur] = image.image[i, j] == noir;
                }
            }
        }

        public static void MultiSquare<T>(T[,] matrice, T[] remplisage, int y, int x)
        {
            int ymax = Math.Min(y + remplisage.Length, matrice.GetLength(0));
            int xmax = Math.Min(x + remplisage.Length, matrice.GetLength(1));
            for (int i = Math.Max(y - remplisage.Length + 1, 0); i < ymax; i++)
            {
                for (int j = Math.Max(x - remplisage.Length + 1, 0); j < xmax; j++)
                {
                    matrice[i, j] = remplisage[Math.Max(Math.Abs(y - i), Math.Abs(x - j))];
                }
            }
        }

        static List<bool> Int_To_Bits(int nombre, int longueur)// entier--> Bits
        {
            List<bool> bits = new List<bool>();
            for (int i = 0; i < longueur; i++)
            {
                bits.Insert(0, ((nombre >> i) & 1) == 1);
            }
            return bits;
        }
        static int Bits_To_Int(List<bool> bits)    // Bits--> entier
        {
            int nombre = 0;
            for (int i = 0; i < bits.Count; i++)
            {
                nombre |= (bits[i] ? 1 : 0) << (bits.Count - i - 1);
            }
            return nombre;
        }
        static byte[] Bits_To_Bytes(List<bool> bits) // Bits--> octets
        {
            byte[] octets = new byte[bits.Count / 8];
            for (int i = 0; i < octets.Length; i++)
            {
                octets[i] = (byte)(Bits_To_Int(bits.GetRange(i * 8, 8)));
            }
            return octets;
        }
        static List<bool> Bytes_To_Bits(byte[] octets) // octets--> Bits
        {
            List<bool> bits = new List<bool>();
            for (int i = 0; i < octets.Length; i++)
            {
                bits.AddRange(Int_To_Bits(octets[i], 8));
            }
            return bits;
        }

        static int[] Assemblage_Tableau(int[] x, int[] y) // tab1+tab2=tab3
        {
            int[] z = new int[x.Length + y.Length];
            x.CopyTo(z, 0);
            y.CopyTo(z, x.Length); return z;
        }

        #endregion TD6
    }
}




