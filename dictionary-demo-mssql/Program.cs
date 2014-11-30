// Пример и тест для библиотеки SQL DictLib http://www.solarix.ru/for_developers/api/dictionary-orm.shtml
//
// Подробнее об SQL словаре: http://www.solarix.ru/for_developers/docs/sql_dictionary.shtml
//
// Внимание: для демо-версии SQL словаря необходимо скорректировать используемые статьи, так
// в его состав входят только небольшое количество слов и фраз.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
//using MySql.Data.MySqlClient;

namespace DictLib
{
 class Program
 {
  static void Main(string[] args)
  {
   // Подключение к словарю, загруженному в MS SQL.
   // http://www.solarix.ru/orm/ru/dal.shtml#mssql
   SqlConnection cnx = new SqlConnection(
                                         "Data Source=localhost;"+
                                         "Initial Catalog=solarix;"+
                                         "Integrated Security=True;"+
                                         "MultipleActiveResultSets=true;"
                                        );
   cnx.Open();
   Solarix.MSSQL_DataAccessLayer dal = new Solarix.MSSQL_DataAccessLayer(cnx);

  

/*
   // Используем реализацию data access layer для MySQL.
   // http://www.solarix.ru/orm/ru/dal.shtml#mysql
   string cnx_string = "server=localhost;user id=root; password=; database=solarix; pooling=false;";
   MySql.Data.MySqlClient.MySqlConnection cnx = new MySql.Data.MySqlClient.MySqlConnection(cnx_string);
   cnx.Open();
   Solarix.MySQL_DataAccessLayer dal = new Solarix.MySQL_DataAccessLayer(cnx);
*/


/*
   System.Data.Odbc.OdbcConnection cnx = new System.Data.Odbc.OdbcConnection("Dsn=solarix_firebird;");
   cnx.Open();
   Solarix.ODBC_DataAccessLayer dal = new Solarix.ODBC_DataAccessLayer(cnx);
*/


/*
   // Подключение к FireBird через FireBird .NET Provider, доступный на официальном сайте
   FirebirdSql.Data.FirebirdClient.FbConnectionStringBuilder cs = new FirebirdSql.Data.FirebirdClient.FbConnectionStringBuilder();
        
   cs.DataSource = "localhost";
   cs.Database = "e:\\db\\solarix.fdb";
   cs.UserID = "SYSDBA";
   cs.Password = "masterkey";
   cs.Dialect = 3;
   cs.Charset = "UTF8";
        
   string cnx_string = cs.ToString();

   FirebirdSql.Data.FirebirdClient.FbConnection cnx = new FirebirdSql.Data.FirebirdClient.FbConnection(cnx_string);
   cnx.Open();
   Solarix.FireBird_DataAccessLayer dal = new Solarix.FireBird_DataAccessLayer(cnx);
*/


/*
   //                              --- ORACLE ---

   // Using ODP.NET without tnsnames.ora:
   // Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=MyHost)(PORT=MyPort)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=MyOracleSID)));User Id=myUsername;Password=myPassword;

   string cnx_string = "Data Source=DIGAMMA;User Id=INKOZIEV;Password=masterkey;";
   Oracle.DataAccess.Client.OracleConnection cnx = new Oracle.DataAccess.Client.OracleConnection(cnx_string);
   cnx.Open();
   Solarix.Oracle_DataAccessLayer dal = new Solarix.Oracle_DataAccessLayer(cnx);
*/


   // Подключаем словарь к БД. Обратите внимание, что это очень легкая (в смысле использования ресурсов)
   // операция - никакой информации в этот момент из БД мы не извлекаем. Необходимые данные запрашиваются
   // из базы по мене необходимости - когда происходит обращение к соответствующим свойствам объектов
   // в Dictionary ORM.
   Solarix.Dictionary dict = new Solarix.Dictionary(dal);


   // вывод списка объявленных в словаре грамматических классов - частей речи.
   // http://www.solarix.ru/orm/ru/PartOfSpeech.shtml
   foreach( Solarix.PartOfSpeech p in dict.partsofspeech )
    {
     Console.WriteLine( "{0} ({1})", p.name, p.language.name );
    }


   // Выведем список наречий, оканчивающихся на -ЩЕ
   // Под MySQL возникает проблема из-за одновременного открытия >1 ридера, это известный
   // баг в .NET провайдере под эту СУБД.
   var adverbs = dict.entries.All().Where( z => (z.name.EndsWith("ще") &&
                                           z.partofspeech.id==SolarixGrammarEngineNET.GrammarEngineAPI.ADVERB_ru) );
   foreach( Solarix.WordEntry e in adverbs )
    {
     Console.WriteLine( "{0} {1}", e.name, e.partofspeech.name );
    }


   // Поиск единственной словарной статьи по ее имени.
   // В случае, если таких статей на самом деле несколько, то
   // найдется какая-то одна из них.
   Solarix.WordEntry кошка = dict.entry["кошка"];


   Console.WriteLine( "id={0} {1} language={2}", кошка.id, кошка, кошка.partofspeech.language.name );

   // Атрибуты у статьи - это набор грамматических признаков, описывающих статью в целом, в отличие
   // от измерений - признаков у каждой из грамматических форм.
   // В случае русского существительного это будет грамматический род, перечислимость и одушевленность.
   Console.WriteLine( " attrs={0}", кошка.attrs );

   // Определим грамматический род:
   Solarix.CoordPair gender = кошка.attrs.FindState(SolarixGrammarEngineNET.GrammarEngineAPI.GENDER_ru);
   Console.WriteLine( " {0}", gender );

   // Каждая словарная статья имеет набор грамматических форм. Даже у неизменяемых частей
   // речи имеется минимум одна форма - собственно базовая, плюс в этом случае может быть несколько
   // альтернативных форм. У русского существительного грамматических форм более десятка в типичном случае.
   // Перечислим их и напечатаем.
   foreach( Solarix.EntryForm ef in кошка.forms )
    {
     Console.WriteLine( " {0}", ef );
    }
 

   // Такие связи у нашей словарной статьи были до манипуляций
   // Обращаю внимание, что это - список всех исходящих связей, если подключен
   // англо-русский словарь, то увидим также английские существительные.
   foreach( Solarix.WordLink l in кошка.links ) 
    {
     Console.WriteLine( " {0}", l );
    }

   // Мы можем очень легко отфильтровать именно переводы, используя тип связи.
   // Тип связи для переводов зависит от языка правой части связи, в случае английского
   // языка будет TO_ENGLISH_link. Переобозначим константу для удобства.
   const int en_transl = SolarixGrammarEngineNET.GrammarEngineAPI.TO_ENGLISH_link;

   // ...и покажем эти связи, предикат для фильтра проверяет у связей свойство type.
   foreach( Solarix.WordLink l in кошка.links.Where( z => z.type==en_transl ) )
    {
     Console.WriteLine( " {0}", l );

     // вот эта словарная статья стоит справа, то бишь это перевод НА английский.
     Solarix.WordEntry en_word = l.right;

     // имя словарной статьи обычно на практике совпадает с некоторой базовой грамматической
     // формой, для английский существительных у нас получается все совсем просто.
     string name = en_word.name;

     Console.WriteLine( "{0}", name );     
    }
   

   Console.WriteLine( "--------------------------------------" );

   // Noun gender determination
   foreach( string noun_str in new string[] { "мышка", "котенок", "животное" } )
    {
     // look up the dictionary and find the word entry
     Solarix.WordEntry noun_entry = dict.entry[noun_str]; 
     
     // get the gender attribute
     Solarix.CoordPair igender = noun_entry.attrs.FindState(SolarixGrammarEngineNET.GrammarEngineAPI.GENDER_ru);

     // print the gender in human readable form
     switch(igender.id_state)
     {
      case SolarixGrammarEngineNET.GrammarEngineAPI.MASCULINE_GENDER_ru:
       Console.WriteLine("{0}: masculine gender", noun_str );
       break;

      case SolarixGrammarEngineNET.GrammarEngineAPI.FEMININE_GENDER_ru:
       Console.WriteLine("{0}: feminine gender", noun_str );
       break;
       
      case SolarixGrammarEngineNET.GrammarEngineAPI.NEUTRAL_GENDER_ru:
       Console.WriteLine("{0}: neuter gender", noun_str );
       break;
     }
    }
   
   Console.WriteLine( "--------------------------------------" );


   // Проверим работу с английской частью лексикона.
   Solarix.WordEntry cat = dict.entry["cat"];
   if( cat!=null )
    {
     // английский лексикон загружен и доступен!
     // посмотрим переводы на русский.
     const int рус_перевод = SolarixGrammarEngineNET.GrammarEngineAPI.TO_RUSSIAN_link;

     // ...и покажем эти связи, предикат для фильтра проверяет у связей свойство type.
     foreach( Solarix.WordLink l in cat.links.Where( z => z.type==рус_перевод ) )
      {
       Console.WriteLine( " {0}", l );
      }
    }



   // Просто для удобства вместо длинного выражения типа связи введем константу с коротким именем.
   const int антоним = SolarixGrammarEngineNET.GrammarEngineAPI.ANTONYM_link;

   // Добавляем связь со статьей "собака". В демо-версии эта статья может отсутствовать.
   Solarix.WordEntry собака = dict.entry["собака"];
   if( собака!=null )
    {
     Solarix.WordLink lnk0 = new Solarix.WordLink( dict, антоним, кошка, собака );
     кошка.links.Add( lnk0 );

     Console.WriteLine( "--------------------------------------" );

     // Проверим теперь список, отфильтровав именно антонимы.
     foreach( Solarix.WordLink l in кошка.links.Where( z => z.type==антоним ) )
      Console.WriteLine( " {0}", l );

     Console.WriteLine( "--------------------------------------" );
   
     // Удалим добавленную связь
     кошка.links.Remove(lnk0);
  
     Console.WriteLine( "--------------------------------------" );
    }

   // Снова проверим список, в нем не должно уже быть этой связи
   foreach( Solarix.WordLink l in кошка.links ) Console.WriteLine( " {0}", l.ToString() );

   // Пройдемся по всем словарным статьям, в которых имена начинаются на КОШ*
   // Список сразу сделаем отсортированным с помощью Linq-конструкции
   foreach( Solarix.WordEntry e in dict.entries.MatchPrefix("кош").OrderBy( z => z.name ) )
    {
     Console.WriteLine( "{0} {1} ", e.name, e.partofspeech.name );

     // Для каждой статьи также выведем в эту же строку список связанных с ней
     foreach( Solarix.WordLink l in e.links )
      {
       Console.WriteLine( " {0}", l );
      }

     Console.WriteLine();
    }


   // Проекция слова на лексикон - находим все грамматические формы, лексически совпадающие
   // с указанным словом. В данном примере мы должны получить три варианта - две формы
   // существительного 'рой' и одну побудительную форму глагола 'рыть'.
   foreach( Solarix.EntryForm ef in dict.forms.Match("рой") )
    {
     Console.WriteLine( "{0}", ef );
    }

   Solarix.PartOfSpeech Сущ = dict.partsofspeech[SolarixGrammarEngineNET.GrammarEngineAPI.NOUN_ru];

   Console.WriteLine( "-------------------------" ); 

   // Чтобы отфильтровать в результатах проекции только существительные, можно конечно использовать
   // LINQ-выражение, но можно использовать и специальный оптимизированный вариант метода поиска, в
   // котором задается грамматический класс (часть речи) искомых форм.
   foreach( Solarix.EntryForm ef in dict.forms.Match("рой",Сущ) )
    {
     Console.WriteLine( "{0}", ef );
    }

   Console.WriteLine( "-------------------------" );

   // теперь поработаем с фразовыми статьями.

   // поиск фразы по ее текстовому содержимому. такая фраза точно есть в нашем англо-русском тезаурусе.
   Solarix.Phrase постоянный_ток = dict.phrase["постоянный ток"];

   if( постоянный_ток!=null )
    {
     Console.WriteLine( "id={0} text={1} class={2}", постоянный_ток.id, постоянный_ток.text, постоянный_ток.partofspeech.name );

     // Проверим, что есть связь с английской фразой "direct current"
     Solarix.Phrase direct_current = dict.phrase["direct current"];
     if( direct_current!=null )
      {
       // есть такая связь?
       int count = direct_current.links.Count( z => z.right==постоянный_ток );
       if( count==1 )
        {
         // есть!
         Console.WriteLine( "count={0}", count );
        }
      }

     // создадим новую фразу
     Solarix.Phrase постоянное_напряж = new Solarix.Phrase( dict, "постоянное напряжение" );
     dict.phrase.Add(постоянное_напряж);

     // создадим связь между двумя фразами
     const int синоним = SolarixGrammarEngineNET.GrammarEngineAPI.SYNONYM_link;
     Solarix.PhraseLink lnk1 = new Solarix.PhraseLink( dict, синоним, постоянный_ток, постоянное_напряж );
     постоянный_ток.links.Add( lnk1 );

     // выведем все связи
     foreach( Solarix.PhraseLink l in постоянный_ток.links )
      {
       Console.WriteLine( " {0}", l );
      }
  

     Console.WriteLine( "-------------------------" ); 

     // Удалим созданные нами связи и фразовые статьи.
     постоянный_ток.links.Remove(lnk1);   
     dict.phrase.Remove(постоянное_напряж);

     // Выведем теперь список связей
     foreach( Solarix.PhraseLink l in постоянный_ток.links )
      {
       Console.WriteLine( " {0}", l );
      }
    }


   // Далее выполним несколько манипуляций со словарем в рамках одной транзакции.
   dal.BeginTx();   

   // Пропробуем создать новую словарную статью. Самый простой случай - английское существительное.
   Solarix.WordEntry xyxyxy = new Solarix.WordEntry(
                                                    dict,
                                                    SolarixGrammarEngineNET.GrammarEngineAPI.NOUN_en,
                                                    "xyxyxy"
                                                   );

   xyxyxy.forms.Add( new Solarix.EntryForm( dict, "xyxyxy" ) );

   // сохраняем созданную статью в БД.
   dict.entries.Add(xyxyxy);

   // ...

   // теперь удалим статью из БД.
   dict.entries.Remove(xyxyxy);

   // Фиксируем все изменения. На самом деле изменений уже никаких нет - мы удалили всю
   // добавленную информацию сами.
   dal.CommitTx();


   dal.BeginTx();

 
   // http://www.solarix.ru/orm/ru/RuNounEntry.shtml
   Solarix.WordEntry хрюндель1 = new Solarix.RuNounEntry(
                                                         dict,
                                                         SolarixGrammarEngineNET.GrammarEngineAPI.MASCULINE_GENDER_ru,
                                                         SolarixGrammarEngineNET.GrammarEngineAPI.ANIMATIVE_FORM_ru,
                                                         SolarixGrammarEngineNET.GrammarEngineAPI.COUNTABLE_ru,
                                                         "хрюндель", "хрюндели",
                                                         "хрюнделя", "хрюнделей",
                                                         "хрюнделем", "", "хрюнделями",
                                                         "хрюнделя", "хрюнделей",
                                                         "хрюнделю", "хрюнделям",
                                                         "хрюнделе", "хрюнделях" );

   dict.entries.Add( хрюндель1 );

   Solarix.WordEntry хрюндель2 = dict.entry["хрюндель"];
   foreach( Solarix.EntryForm f in хрюндель2.forms )
    {
     Console.WriteLine( "{0}", f.name );
    }


   // Инфинитив - неопределенная форма глагола - объявляется как отдельная словарная статья.
   // http://www.solarix.ru/orm/ru/RuInfEntry.shtml
   Solarix.WordEntry чтотоделать = new Solarix.RuInfEntry(
                                                          dict,
                                                          SolarixGrammarEngineNET.GrammarEngineAPI.IMPERFECT_ru,
                                                          SolarixGrammarEngineNET.GrammarEngineAPI.TRANSITIVE_VERB_ru,
                                                          "вин",
                                                          "чтотоделать"
                                                         );

   dict.entries.Add(чтотоделать);

   // Личная форма глагола
   // http://www.solarix.ru/orm/ru/RuVerbEntry.shtml
   Solarix.WordEntry гл_чтотоделать = new Solarix.RuVerbEntry(
                                                              dict,
                                                              SolarixGrammarEngineNET.GrammarEngineAPI.IMPERFECT_ru,
                                                              SolarixGrammarEngineNET.GrammarEngineAPI.TRANSITIVE_VERB_ru,
                                                              "вин",
                                                              "чтотоделать",
                                                              "чтотоделаю", "чтотоделаем",
                                                              "чтотоделаешь", "чтотоделаете",
                                                              "чтотоделает", "чтотоделают",
                                                              "чтотоделал", "чтотоделала", "чтотоделало", "чтотоделали",
                                                              "чтотоделай", "чтотоделайте", ""
                                                             );

   dict.entries.Add(гл_чтотоделать);

   // Прилагательное
   // http://www.solarix.ru/orm/ru/RuAdjEntry.shtml
   Solarix.WordEntry глоздый = new Solarix.RuAdjEntry( dict,
                                "глоздый", "глоздая", "глоздое", "глоздые",
                                "глоздого", "глоздой", "глоздого", "глоздых",
                                "глоздым", "глоздой", "глоздым", "глоздыми",   
                                "глоздого", "глоздую", "глоздое", "глоздых",
                                "глоздый",                        "глоздые",   
                                "глоздому", "глоздой", "глоздому", "глоздым",
                                "глоздом", "глоздой", "глоздом", "глоздых",
                                new Solarix.RuAdjShort( "глозд", "глозда", "глоздо", "глозды" ),
                                new Solarix.RuAdjCompar( "глоздее" ),
                                new Solarix.RuAdjSuper( "глоздейший", "глоздейшая", "глоздейшее", "глоздейшие",
                                                        "глоздейшего", "глоздейшей", "глоздейшего", "глоздейших",
                                                        "глоздейшим", "глоздейшей", "глоздейшим", "глоздейшими",
                                                        "глоздейшего", "глоздейшую", "глоздейшее", "глоздейших",
                                                        "глоздейший",                              "глоздейшие",
                                                        "глоздейшему", "глоздейшей", "глоздейшему", "глоздейшим",
                                                        "глоздейшем", "глоздейшей", "глоздейшем", "глоздейших"
                                   ) );
   dict.entries.Add(глоздый);

                    
   // Наречие 
   // http://www.solarix.ru/orm/ru/RuAdvEntry.shtml
   Solarix.WordEntry кавайно = new Solarix.RuAdvEntry( dict, "кавайно", "кавайнее" );
   dict.entries.Add( кавайно );

   // Причастие
   // http://www.solarix.ru/orm/ru/RuParticipleEntry.shtml
   Solarix.WordEntry хрюндящий = new Solarix.RuParticipleEntry( dict,
    SolarixGrammarEngineNET.GrammarEngineAPI.IMPERFECT_ru,
    SolarixGrammarEngineNET.GrammarEngineAPI.NONTRANSITIVE_VERB_ru,
    SolarixGrammarEngineNET.GrammarEngineAPI.PRESENT_ru,
    false,
    "", // список падежей для падежной валентности
                                "хрюндящий", "хрюндящая", "хрюндящее", "хрюндящие",
                                "хрюндящего", "хрюндящей", "хрюндящего", "хрюндящих",
                                "хрюндящим", "хрюндящей", "хрюндящим", "хрюндящими",   
                                "хрюндящего", "хрюндящую", "хрюндящее", "хрюндящих",
                                "хрюндящий",                        "хрюндящие",   
                                "хрюндящему", "хрюндящей", "хрюндящему", "хрюндящим",
                                "хрюндящем", "хрюндящей", "хрюндящем", "хрюндящих" );

   dict.entries.Add(хрюндящий);

   // Деепричастие
   // http://www.solarix.ru/orm/ru/RuAdvParticipleEntry.shtml
   Solarix.WordEntry хрюндя = new Solarix.RuAdvParticipleEntry( dict,
    SolarixGrammarEngineNET.GrammarEngineAPI.IMPERFECT_ru,
    SolarixGrammarEngineNET.GrammarEngineAPI.NONTRANSITIVE_VERB_ru,
    "вин;твор", // список падежей для падежной валентности
    "хрюндя" );

   dict.entries.Add(хрюндя);


   // тут можно поработать с созданными словарными статьями.
   // ...

   // теперь поудаляем все словарные статьи, которые мы только что добавили.
   dict.entries.Remove(хрюндель1);
   dict.entries.Remove(чтотоделать);
   dict.entries.Remove(гл_чтотоделать);
   dict.entries.Remove(глоздый);
   dict.entries.Remove(кавайно);
   dict.entries.Remove(хрюндящий);
   dict.entries.Remove(хрюндя);

   dal.CommitTx();
   
   cnx.Close();
   return;
  }
 }
}
