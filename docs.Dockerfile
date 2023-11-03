FROM mcr.microsoft.com/dotnet/sdk:7.0 
COPY . /indico-client-csharp
WORKDIR /indico-client-csharp
RUN chmod +x ./scripts/build_docs.sh
CMD ["sleep", "infinity"]

# ARG COMMIT_MSG=${git log -1 --pretty=%B}
# # use devops sa github pat to interact with readme docs repo
# WORKDIR /
# RUN git clone git@github.com:IndicoDataSolutions/indico-readme.git
# RUN git checkout -b csharp-docs
# COPY /indico-client-csharp/_site /indico-readme/sdks/csharp
# RUN git add --all && git commit -m ${COMMIT_MSG} && git push -u origin csharp-docs



