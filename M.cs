/*
COMPRESSION ALGORITHM FOR A SOURCE FILE THAT IS WRITTEN IN DECIMAL/BASE10				7/8/2022
Source File:             1234679821341653496101976534354799739999999999991234567891234597...
Parsing (12-digit):      123467982134 165349610197 653435479973 999999999999 123456789123 4597...



#toStore s                                 s =          999,999,999,999 = 11101000 11010100 10100101 00001111 11111111 = 40 bits
                                                                    Compressed # MUST be smaller than ^ for "compression" to be useful
s^(1/3) = b                                b =         (999,999,999,999)^(1/3) = 9,999.9  ==>  Rounds up to 10,000, creating a smaller offset than rounding down VERIFY
round(b)^3 = e                             e = 10,000^3 = 1,000,000,000,000
#toStore-exponentiated# = signedOffset      o = s-e = 999,999,999,999-1,000,000,000,000 = -1 = 11 = 2 bits
#toStore = b^3+signedOffset
bbbbbbbbb    oooooooo oooooooo oooooooo oooooooo oooooooo
7MAX         1,099,511,627,776 unsignedMAX
Why 3 as the exponent? Because it's the max # that can be stored in 2 bits, AND smaller exponents mean smaller offsets, meaning less bits to store the offset
How many decimal digits should s be read as?  1)Comma readability 2)Big sourceFile #s make compression more efficient.   12 decimal digits


40 bits: sourceFile# in Base10
42 bits: Compressed with exponent=15, sourceFileNumSize=12 digits, maxOffsetSize=40 bits.   OffsetBits+exponentBaseBits=40+3=43
43>=40, so this Compression algorithm doesn't work.





#toStore s                                 s =          999,999,999,999 = 11101000 11010100 10100101 00001111 11111111 = 40 bits
                                                                    Compressed # MUST be smaller than ^ for "compression" to be useful
s^(1/7) = x                                x =         (999,999,999,999)^(1/7) = 51.7947  ==>  Rounds up to 52, creating a smaller offset than rounding up VERIFY
round(x)^7 = e                             e = 52^7 = 1,028,071,702,528
#toStore-exponentiated# = signedOffset     o = s-e = 999,999,999,999-1,028,071,702,528 = -28,071,702,529 = neg(110 10001001 00110100 00110000 00000001) = 36 bits
#toStore = x^7+signedOffset
xxxx xx    oooo oooooooo oooooooo oooooooo oooooooo
63MAX      68,719,476,736 unsignedMAX
Why 7 as the exponent? Because it's the max # that can be stored in 3 bits, AND smaller exponents mean smaller offsets, meaning less bits to store the offset
How many decimal digits should s be read as?  1)Comma readability 2)Big sourceFile #s make compression more efficient.   12 decimal digits

40 bits: sourceFile# in Base10
42 bits: Compressed with exponent=7, sourceFileNumSize=12 digits, maxOffsetSize=36 bits.   OffsetBits+exponentBASEbits=36+6=42
42>=40, so this Compression algorithm doesn't work.



#toStore s                                  s =          999,999,999,999 = 11101000 11010100 10100101 00001111 11111111 = 40 bits
                                                                    Compressed # MUST be smaller than ^ for "compression" to be useful
s^(1/15) = b                                b =         (999,999,999,999)^(1/15) = 6.3095  ==>  Rounds down to 6, creating a smaller offset than rounding down VERIFY
round(b)^15 = e                             e = 6^15 =   470,184,984,576
#toStore-exponentiated# = signedOffset      o = s-e = 999,999,999,999-470,184,984,576 = 529,815,015,423 = 1111011 01011011 01101111 10001111 11111111 = 40 bits
#toStore = b^15+signedOffset
bbb    oooooooo oooooooo oooooooo oooooooo oooooooo
7MAX   1,099,511,627,776 unsignedMAX
Why 15 as the exponent? Because it's the max # that can be stored in 4 bits, AND smaller exponents mean smaller offsets, meaning less bits to store the offset
How many decimal digits should s be read as?  1)Comma readability 2)Big sourceFile #s make compression more efficient.   12 decimal digits


40 bits: sourceFile# in Base10
42 bits: Compressed with exponent=15, sourceFileNumSize=12 digits, maxOffsetSize=40 bits.   OffsetBits+exponentBaseBits=40+3=43
43>=40, so this Compression algorithm doesn't work.



*/
using System;
public class Compress
{
	//ALL OF BELOW NEED NOT BE CONTINUED BECAUSE NUMERICAL BASES ARE PERFECTLY EFFICIENT AT LOSSLESS COMPRESSION
	//Using exponents to compress is most efficient when a # raised to a power is NOT exclusive to one digit.
	//E.g. 10^x is NOT efficient because it only affects 1 digit, either 10 or 100 or 1,000 or 10,000 etc.
	//Compared to 10^x, 3^x is more efficient (though it needs bigger exponents and hence more bits)
		//because it alters multiple digits with every raised exponent, either 27 or 81 or 243 or 729 or 2,187 or 6,561 or 19,683 or 59,049 etc
	//tl;dr TO COMPRESS WITH EXPONENTIATION, AVOID USING THE BASE THE COMPRESSOR TAKES AS INPUT. For a file written in Base10, avoid using 10^x for compressing the data
	//HOWEVER, 7^x is more efficient than 3^x because it retains 3^x's nonexclusive property AND takes lesser exponents to get large #s

	//longs are limited to 18 Base10 '9' digits (uses 60 out of 63 positive bits, the 64th being a negative bit),
	//so any # with all '9's longer than 18 digits is useless without using BigInteger
	
	//Compress_2term(numToCompress):	num = x^y+offset				82  =  2^6 + 18                 =  3^4 + 1  =  4^3 + 18
	//Compress_3term(numToCompress):	num = a^b+c^d+offset			82  =  2^6 + 3^2 + 9
	//Compress_4term(numToCompress):	num = a^b+c^d+e^f+offset		82  =  2^6 + 3^2 + 5^1 + 4
	//Compress_5term(numToCompress):	num = a^b+c^d+e^f+g^h+offset	82  =  2^6 + 3^2 + 5^1 + 7^0 + 3

	//Compress_2term(numToCompress):	num = x^y+offset				82 = 2^6+18 = 3^4+1 = 4^3+18
	public long[] Compress_2term(long numToCompress_b10)
	{
		//TRY THIS RECURSIVELY WHERE A # FEEDS BACK INTO ITSELF TO HAVE A SMALLER OFFSET.
		//E.g. 17^12+11^3+173645 instead of 17^12+393000
		//Led to Compress_xterm(numToCompress), where instead of finding a base AND an exponent, only find an exponent to save bits
		
		int numToCompress_b10_numDigits = numToCompress_b10.ToString().Length;		//Base10#->String->LengthOfString
		string srcFileNum_bin  = Convert.ToString(numToCompress_b10,2);				//Converts Base10(Decimal)# to Base2(Binary)#
		int srcFileNum_numBits = srcFileNum_bin.Length;								//Gets #ofBits in the Base2#

		long[] winningCombo = {long.MaxValue,long.MaxValue,long.MaxValue};			//{baseNum, exponent, offset}
		for(int exponent=2; exponent<=25; exponent++)
		//LowerBound=2 because 0 and 1 are useless. 25 is kind of arbitrary but relatively small
		//(big exponents yield no benefit because they make extremely small bases like 2 and 3, meaning huge offsets are needed, which
		//take more bits).
		{
			double root = 1.0/exponent;				//square, cube, etc root

			//(int) truncation acts like Math.Floor(#), meaning I don't have to deal with negative #s, meaning I can use unsigned #s and save 1 bit
			int baseNum = (int)( Math.Pow(numToCompress_b10,root) );			//sqr/cb/etc root of srcFile#InBase10
			int baseNum_numBits = Convert.ToString(baseNum,2).Length;		//Base10#->Base2#->StringOfBase2#s->LengthOfString
			long exponentiatedNum = (long)( Math.Pow(baseNum,exponent) );	//baseNum^exponent
			long offset = numToCompress_b10-exponentiatedNum;
			int offset_numBits = Convert.ToString(offset,2).Length;			//Base10#->Base2#->StringOfBase2#s->LengthOfString
			int totalBitsNeededForCompression = baseNum_numBits+offset_numBits;
			if(totalBitsNeededForCompression<srcFileNum_numBits && totalBitsNeededForCompression<(winningCombo[0]+winningCombo[2]))//if(compressionIsUseful){Update Array to new {baseNum, exponent, offset} }
			{
				winningCombo[0] = baseNum;
				winningCombo[1] = exponent;
				winningCombo[2] = offset;
			}
			Console.WriteLine($"WINNING\nsrcFile#: {numToCompress_b10}({numToCompress_b10_numDigits} digits,{srcFileNum_numBits} bits), exponent: {winningCombo[1]}, baseOfExponent: {winningCombo[0]}({Convert.ToString(winningCombo[0],2).Length} bits), offset: {winningCombo[2]}({Convert.ToString(winningCombo[2],2).Length} bits)");
			Console.WriteLine($"srcFile#: {numToCompress_b10}({numToCompress_b10_numDigits} digits,{srcFileNum_numBits} bits), exponent: {exponent}, baseOfExponent: {baseNum}({baseNum_numBits} bits), offset: {offset}({offset_numBits} bits)");
			Console.WriteLine($"srcFile#: {numToCompress_b10} => {baseNum}^{exponent} + {offset} = {totalBitsNeededForCompression} bits\n");
		}
		Console.WriteLine("\n\n");	//Three newlines
		return winningCombo;
	}



	
	//Compress_3term(numToCompress):	num = a^b+c^d+offset			84  =  3^4 + 2^1 + 1
	public long[] Compress_3term(long numToCompress_b10)
	{
		//1) Figure out how many powers of 3 can fit in numToCompress
		//2) After fitting in every power of 3, fit all possible powers of 2 into the remainder
		//3) After fitting in every power of 3 and then 2, make the rest the remainder(offset)

		//(int) truncation acts like Math.Floor(#), meaning I don't have to deal with negative #s, meaning I can use unsigned #s and save 1 bit
		//numToCompress = 3^x   =>   log(#toCompress) = x*log(3)   =>   x = log(#toCompress)/log(3)
		int  exponent1OfBaseNum3     = (int)( Math.Log(numToCompress_b10)/Math.Log(3) );		//exponent = log(#toCompress)/log(3)
		long exponentiated1_BaseNum3 = (long)( Math.Pow(3/*BaseNum*/,exponent1OfBaseNum3) );	//3^exponent
		long remainder1FromBaseNum3  = numToCompress_b10 - exponentiated1_BaseNum3;				//E.g. remainder = 84 - 3^4

		//remainderFromBaseNum3 = numToCompress - 3^x
		//remainderFromBaseNum3 = 2^y   =>   log(remainderFromBaseNum3) = y*log(2)   =>   y = log(remainderFromBaseNum3)/log(2)
		int  exponentOfBaseNum2    = (int)( Math.Log(remainder1FromBaseNum3)/Math.Log(2) );		//exponent = log(remainderFromBaseNum3)/log(2)
		long exponentiatedBaseNum2 = (long)( Math.Pow(2/*BaseNum*/,exponentOfBaseNum2) );		//2^exponent
		long remainderFromBaseNum2 = (numToCompress_b10 - exponentiated1_BaseNum3) - exponentiatedBaseNum2;		//E.g. newRemainder = (84 - 3^4) - 2^1
		long offset = remainderFromBaseNum2;


		int numToCompress_b10_numDigits = numToCompress_b10.ToString().Length;	//Base10(Decimal)#->String->LengthOfString

		//Base10(Decimal)#->Base2(Binary)#->StringOfBase2#s->LengthOfString. Gets #ofBits in the Base2#
		int numToCompress_numBits      = Convert.ToString(numToCompress_b10,2).Length;
		int exponentOfBaseNum3_numBits = Convert.ToString(exponent1OfBaseNum3,2).Length;
		int exponentOfBaseNum2_numBits = Convert.ToString(exponentOfBaseNum2,2).Length;
		int offset_numBits             = Convert.ToString(offset,2).Length;

		int totalBitsNeededForCompression = exponentOfBaseNum3_numBits+exponentOfBaseNum2_numBits+offset_numBits;
		if(totalBitsNeededForCompression<numToCompress_numBits)//if(compressionIsUseful){Print "Compression was useful"}
			{Console.WriteLine($"totalBitsNeededForCompression<srcFileNum_numBits: {totalBitsNeededForCompression}<{numToCompress_numBits}");}
		Console.WriteLine($"srcFile#: {numToCompress_b10}({numToCompress_b10_numDigits} digits,{numToCompress_numBits} bits), exponentOfBaseNum3: {exponent1OfBaseNum3}({exponentOfBaseNum3_numBits} bits), exponentOfBaseNum2: {exponentOfBaseNum2}({exponentOfBaseNum2_numBits} bits), offset: {offset}({offset_numBits} bits)");
		Console.WriteLine($"srcFile#: {numToCompress_b10} => 3^{exponent1OfBaseNum3} + 2^{exponentOfBaseNum2} + {offset} = {totalBitsNeededForCompression} bits\n");
		Console.WriteLine("\n\n");	//Three newlines

		return new long[]{exponent1OfBaseNum3, exponentOfBaseNum2, offset};
	}


	//FIX ME    FINISH THIS
	//Compress_4term(numToCompress):	num = a^b+c^d+e^f+offset			 82  =  2^6 + 3^2 + 5^1 + 4		NAY, I WANT SAME BASE# WITH DIFFERENT EXPONENTS
	//Compress_4term(numToCompress):	num = a^b+a^c+a^d+offset		982,764(20b)  =  13^5 + 13^5 + 13^4 + 211,617 (3b+3b+3b+18b)
	public long[] Compress_4term(long numToCompress_b10)
	{
		//1) Figure out how many powers of 3 can fit in numToCompress
		//2) After fitting in every power of 3, fit all possible powers of 2 into the remainder
		//3) After fitting in every power of 3 and then 2, make the rest the remainder(offset)

		//(int) truncation acts like Math.Floor(#), meaning I don't have to deal with negative #s, meaning I can use unsigned #s and save 1 bit
		//numToCompress = 3^x   =>   log(#toCompress) = x*log(3)   =>   x = log(#toCompress)/log(3)
		int  exponent1OfBaseNum3     = (int)( Math.Log(numToCompress_b10)/Math.Log(3) );		//exponent = log(#toCompress)/log(3)
		long exponentiated1_BaseNum3 = (long)( Math.Pow(3/*BaseNum*/,exponent1OfBaseNum3) );	//3^exponent
		long remainder1FromBaseNum3  = numToCompress_b10 - exponentiated1_BaseNum3;				//E.g. remainder = 84 - 3^4

		//remainderFromBaseNum3 = numToCompress - 3^x
		//remainderFromBaseNum3 = 2^y   =>   log(remainderFromBaseNum3) = y*log(2)   =>   y = log(remainderFromBaseNum3)/log(2)
		int  exponentOfBaseNum2    = (int)( Math.Log(remainder1FromBaseNum3)/Math.Log(2) );		//exponent = log(remainderFromBaseNum3)/log(2)
		long exponentiatedBaseNum2 = (long)( Math.Pow(2/*BaseNum*/,exponentOfBaseNum2) );		//2^exponent
		long remainderFromBaseNum2 = (numToCompress_b10 - exponentiated1_BaseNum3) - exponentiatedBaseNum2;		//E.g. newRemainder = (84 - 3^4) - 2^1
		long offset = remainderFromBaseNum2;


		int numToCompress_b10_numDigits = numToCompress_b10.ToString().Length;	//Base10(Decimal)#->String->LengthOfString

		//Base10(Decimal)#->Base2(Binary)#->StringOfBase2#s->LengthOfString. Gets #ofBits in the Base2#
		int numToCompress_numBits      = Convert.ToString(numToCompress_b10,2).Length;
		int exponentOfBaseNum3_numBits = Convert.ToString(exponent1OfBaseNum3,2).Length;
		int exponentOfBaseNum2_numBits = Convert.ToString(exponentOfBaseNum2,2).Length;
		int offset_numBits             = Convert.ToString(offset,2).Length;

		int totalBitsNeededForCompression = exponentOfBaseNum3_numBits+exponentOfBaseNum2_numBits+offset_numBits;
		if(totalBitsNeededForCompression<numToCompress_numBits)//if(compressionIsUseful){Print "Compression was useful"}
			{Console.WriteLine($"totalBitsNeededForCompression<srcFileNum_numBits: {totalBitsNeededForCompression}<{numToCompress_numBits}");}
		Console.WriteLine($"srcFile#: {numToCompress_b10}({numToCompress_b10_numDigits} digits,{numToCompress_numBits} bits), exponentOfBaseNum3: {exponent1OfBaseNum3}({exponentOfBaseNum3_numBits} bits), exponentOfBaseNum2: {exponentOfBaseNum2}({exponentOfBaseNum2_numBits} bits), offset: {offset}({offset_numBits} bits)");
		Console.WriteLine($"srcFile#: {numToCompress_b10} => 3^{exponent1OfBaseNum3} + 2^{exponentOfBaseNum2} + {offset} = {totalBitsNeededForCompression} bits\n");
		Console.WriteLine("\n\n");	//Three newlines

		return new long[]{exponent1OfBaseNum3, exponentOfBaseNum2, offset};
	}
	//Compress_5term(numToCompress):	num = a^b+c^d+e^f+g^h+offset	82  =  2^6 + 3^2 + 5^1 + 7^0 + 3



	public static void Main()
	{
		Console.WriteLine(long.MaxValue);

		Compress c = new Compress();

		String numToCompressAsString = "";
		for(int numLength=1; numLength<19; numLength++)
		{
			numToCompressAsString += "9";
			long numToCompress_b10 = -1;
			bool parseFailed = long.TryParse(numToCompressAsString, out numToCompress_b10);	//StringOfDigits->Digits
			//c.Compress_2term( (c.Compress_2term(numToCompress_b10))[2] );		//Greedy algorithm with 2 lvls of recursion
			c.Compress_3term( numToCompress_b10 );
		}
	}
}