abp install-libs

cd src/HIS.DbMigrator && dotnet run && cd -


cd src/HIS.HttpApi.Host && dotnet dev-certs https -v -ep openiddict.pfx -p config.auth_server_default_pass_phrase 



exit 0