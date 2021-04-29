using System;
using System.Collections.Generic;
using System.Text;

namespace Projet_PSI
{
    class Pixel//tableau contenant les pixels RGB
    {
        byte rouge;
        byte vert;
        byte bleu;
        // les propriétés sont nécessaires pourla classe MyImage
        public byte Rouge
        {
            get{ return rouge;}
            set{rouge = value;}
        }
        public byte Vert
        {
            get{return vert;}
            set{vert = value;}
        }
        public byte Bleu
        {
            get{return bleu;}
            set{bleu = value;}
        }

        public Pixel(int rouge , int vert , int bleu )// constructeur de la classe Pixel
        {
            // en cas de saturation, permet de faire en sorte  que les couleurs soient toujours entre 0 et 255
            if (rouge <= 0) this.rouge = 0;
            else if (rouge >= 255) this.rouge = 255;
            else this.rouge = (byte)(rouge);

            if (vert <= 0) this.vert = 0;
            else if (vert >= 255) this.vert = 255;
            else this.vert = (byte)(vert);

            if (bleu <= 0) this.bleu = 0;
            else if (bleu >= 255) this.bleu = 255;
            else this.bleu = (byte)(bleu); 
        }
        public Pixel(byte rouge, byte vert, byte bleu)// constructeur de la classe Pixel
        {
            this.rouge = rouge;
            this.vert = vert;
            this.bleu = bleu;
        }
        public Pixel(Pixel pixel)// constructeur de la classe Pixel
        {
            this.rouge = pixel.rouge;
            this.vert = pixel.vert;
            this.bleu = pixel.bleu;
        }

        public override string ToString()// utilisé durant la programmation, pour verifier que tout fonctionne
        {
            return rouge + " " + vert + " " + bleu;
        }
        
        public bool PlusProcheDeQue(Pixel a,Pixel b)
        {
            int i=Math.Abs(rouge-a.Rouge)+ Math.Abs(vert - a.Vert)+ Math.Abs(bleu - a.Bleu)-(Math.Abs(rouge - b.Rouge) + Math.Abs(vert - b.Vert) + Math.Abs(bleu - b.Bleu));
            return i<0;
        }
    }
}
