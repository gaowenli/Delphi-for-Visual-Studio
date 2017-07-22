/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
 * C# Flex 1.4                                                             *
 * Copyright (C) 2004-2005  Jonathan Gilbert <logic@deltaq.org>            *
 * Derived from:                                                           *
 *                                                                         *
 *   JFlex 1.4                                                             *
 *   Copyright (C) 1998-2004  Gerwin Klein <lsf@jflex.de>                  *
 *   All rights reserved.                                                  *
 *                                                                         *
 * This program is free software; you can redistribute it and/or modify    *
 * it under the terms of the GNU General Public License. See the file      *
 * COPYRIGHT for more information.                                         *
 *                                                                         *
 * This program is distributed in the hope that it will be useful,         *
 * but WITHOUT ANY WARRANTY; without even the implied warranty of          *
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the           *
 * GNU General Public License for more details.                            *
 *                                                                         *
 * You should have received a copy of the GNU General Public License along *
 * with this program; if not, write to the Free Software Foundation, Inc., *
 * 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA                 *
 *                                                                         *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

using System;

namespace CSFlex
{


/**
 * Stores a regular expression of rules section in a C# Flex specification.
 *
 * This base class has no content other than its type. 
 *
 * @author Gerwin Klein
 * @version JFlex 1.4, $Revision: 2.3 $, $Date: 2004/04/12 10:07:47 $
 * @author Jonathan Gilbert
 * @version CSFlex 1.4
 */
public class RegExp {
  
  /**
   * The type of the regular expression. This field will be
   * filled with values from class sym.java (generated by cup)
   */
  internal int type;
  

  /**
   * Create a new regular expression of the specified type.
   *
   * @param type   a value from the cup generated class sym.
   *
   * @see CSFlex.sym
   */
  public RegExp(int type) {
    this.type = type;    
  }

  

  /**
   * Returns a String-representation of this regular expression
   * with the specified indentation.
   *
   * @param tab   a String that should contain only space characters and
   *              that is inserted in front of standard String-representation
   *              pf this object.
   */
  public virtual String print(String tab) {
    return tab+ToString();
  }


  /**
   * Returns a String-representation of this regular expression
   */
  public override String ToString() {
    return "type = "+type;
  }


  /**
   * Find out if this regexp is a char class or equivalent to one.
   * 
   * @param  macros  for macro expansion
   * @return true if the regexp is equivalent to a char class.
   */
  public bool isCharClass(Macros macros) {
    RegExp1 unary;
    RegExp2 binary;

    switch (type) {
    case sym.CHAR:
    case sym.CHAR_I:
    case sym.CCLASS:
    case sym.CCLASSNOT:
      return true;
      
    case sym.BAR: 
      binary = (RegExp2) this;
      return binary.r1.isCharClass(macros) && binary.r2.isCharClass(macros);
 
    case sym.MACROUSE:
      unary = (RegExp1) this;
      return macros.getDefinition((String) unary.content).isCharClass(macros);
     
    default: return false; 
    }     
  }
  
  /**
   * The approximate number of NFA states this expression will need (only 
   * works correctly after macro expansion and without negation)
   * 
   * @param macros  macro table for expansion   
   */
  public int size(Macros macros) {
    RegExp1 unary;
    RegExp2 binary;
    RegExp content;

    switch ( type ) {
    case sym.BAR: 
      binary = (RegExp2) this;
      return binary.r1.size(macros) + binary.r2.size(macros) + 2;

    case sym.CONCAT:   
      binary = (RegExp2) this;
      return binary.r1.size(macros) + binary.r2.size(macros);
      
    case sym.STAR:
      unary = (RegExp1) this;
      content = (RegExp) unary.content;      
      return content.size(macros) + 2;

    case sym.PLUS:
      unary = (RegExp1) this;
      content = (RegExp) unary.content;      
      return content.size(macros) + 2;
      
    case sym.QUESTION: 
      unary = (RegExp1) this;
      content = (RegExp) unary.content;      
      return content.size(macros);

    case sym.BANG:
      unary = (RegExp1) this;
      content = (RegExp) unary.content;      
      return content.size(macros) * content.size(macros);
      // this is only a very rough estimate (worst case 2^n)
      // exact size too complicated (propably requires construction)
      
    case sym.TILDE:
      unary = (RegExp1) this;
      content = (RegExp) unary.content;      
      return content.size(macros) * content.size(macros) * 3;
      // see sym.BANG
      
    case sym.STRING:
    case sym.STRING_I:
      unary = (RegExp1) this;
      return ((String) unary.content).Length+1;

    case sym.CHAR:
    case sym.CHAR_I:
      return 2;

    case sym.CCLASS:
    case sym.CCLASSNOT:
      return 2;

    case sym.MACROUSE:
      unary = (RegExp1) this;
      return macros.getDefinition((String) unary.content).size(macros);
    }

    throw new Exception("unknown regexp type "+type);
  }
}
}