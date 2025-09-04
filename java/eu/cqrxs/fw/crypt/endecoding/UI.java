package cqrxs.eu.fw.crypt.endecoding;

import java.util.Scanner;

public class UI {

    public static void main(String[] args) {
        var scanner = new Scanner(System.in);
        while (true) {
            printMenu();
            var str = scanner.nextLine();
            System.out.print("Enter string: ");
            switch (str) {
                case "0":
                case "1":
                    System.out.println(new UuCoder().encode(scanner.nextLine()));
                    break;
                case "2":
                case "3":
                    System.out.println(new Base64Coder().encode(scanner.nextLine()));
                    break;
                default:
                    scanner.close();
                    return;
            }
        }
    }

    static void printMenu() {
        System.out.println("=============================================");
        System.out.println("1. Uuencoding");
        System.out.println("2. Xxencoding");
        System.out.println("3. Base64 encoding");
        System.out.println("=============================================");
        System.out.print("> Choose an option: ");
    }
}
