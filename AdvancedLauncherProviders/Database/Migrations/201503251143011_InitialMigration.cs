namespace AdvancedLauncher.Providers.Database.Migrations {

    using System.Data.Entity.Migrations;

    public partial class InitialMigration : DbMigration {

        public override void Up() {
            CreateTable(
                "dbo.Digimons",
                c => new {
                    Id = c.Long(nullable: false, identity: true),
                    TamerId = c.Long(nullable: false),
                    Name = c.String(nullable: false, maxLength: 4000),
                    TypeId = c.Long(nullable: false),
                    Rank = c.Long(nullable: false),
                    Level = c.Byte(nullable: false),
                    SizeCm = c.Double(nullable: false),
                    SizePc = c.Int(nullable: false),
                    SizeRank = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tamers", t => t.TamerId, cascadeDelete: true)
                .ForeignKey("dbo.DigimonTypes", t => t.TypeId)
                .Index(t => t.TamerId)
                .Index(t => t.TypeId);

            CreateTable(
                "dbo.Tamers",
                c => new {
                    Id = c.Long(nullable: false, identity: true),
                    AccountId = c.Int(nullable: false),
                    GuildId = c.Long(nullable: false),
                    Name = c.String(nullable: false, maxLength: 4000),
                    Level = c.Byte(nullable: false),
                    Rank = c.Long(nullable: false),
                    TypeId = c.Long(),
                    IsMaster = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Guilds", t => t.GuildId, cascadeDelete: true)
                .ForeignKey("dbo.TamerTypes", t => t.TypeId)
                .Index(t => t.GuildId)
                .Index(t => t.TypeId);

            CreateTable(
                "dbo.Guilds",
                c => new {
                    Id = c.Long(nullable: false, identity: true),
                    Identifier = c.Int(nullable: false),
                    ServerId = c.Long(nullable: false),
                    Name = c.String(nullable: false, maxLength: 4000),
                    Rep = c.Long(nullable: false),
                    Rank = c.Long(nullable: false),
                    UpdateTime = c.DateTime(),
                    IsDetailed = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Servers", t => t.ServerId, cascadeDelete: true)
                .Index(t => t.ServerId);

            CreateTable(
                "dbo.Servers",
                c => new {
                    Id = c.Long(nullable: false, identity: true),
                    Identifier = c.Byte(nullable: false),
                    Name = c.String(nullable: false, maxLength: 25),
                    Type = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.TamerTypes",
                c => new {
                    Id = c.Long(nullable: false, identity: true),
                    Code = c.Int(nullable: false),
                    Name = c.String(nullable: false, maxLength: 50),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.DigimonTypes",
                c => new {
                    Id = c.Long(nullable: false, identity: true),
                    Code = c.Int(nullable: false),
                    IsStarter = c.Boolean(nullable: false),
                    SizeCm = c.Double(nullable: false),
                    Name = c.String(nullable: false, maxLength: 50),
                    NameAlt = c.String(maxLength: 50),
                    NameKorean = c.String(maxLength: 50),
                    SearchGDMO = c.String(maxLength: 50),
                    SearchKDMO = c.String(maxLength: 50),
                })
                .PrimaryKey(t => t.Id);
        }

        public override void Down() {
            DropForeignKey("dbo.Digimons", "TypeId", "dbo.DigimonTypes");
            DropForeignKey("dbo.Digimons", "TamerId", "dbo.Tamers");
            DropForeignKey("dbo.Tamers", "TypeId", "dbo.TamerTypes");
            DropForeignKey("dbo.Tamers", "GuildId", "dbo.Guilds");
            DropForeignKey("dbo.Guilds", "ServerId", "dbo.Servers");
            DropIndex("dbo.Guilds", new[] { "ServerId" });
            DropIndex("dbo.Tamers", new[] { "TypeId" });
            DropIndex("dbo.Tamers", new[] { "GuildId" });
            DropIndex("dbo.Digimons", new[] { "TypeId" });
            DropIndex("dbo.Digimons", new[] { "TamerId" });
            DropTable("dbo.DigimonTypes");
            DropTable("dbo.TamerTypes");
            DropTable("dbo.Servers");
            DropTable("dbo.Guilds");
            DropTable("dbo.Tamers");
            DropTable("dbo.Digimons");
        }
    }
}